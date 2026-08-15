using ECommerce.Api.Middleware;
using ECommerce.Application;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using Microsoft.Extensions.FileProviders;
using ECommerce.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddScoped<UploadedImageStorage>();

var authAuthority = builder.Configuration["Authentication:Authority"]
    ?? "http://localhost:8080/realms/nexrig";
var authAudience = builder.Configuration["Authentication:Audience"] ?? "nexrig-web";
var authBackchannelAuthority = builder.Configuration["Authentication:BackchannelAuthority"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authAuthority;
        options.Audience = authAudience;
        options.RequireHttpsMetadata = builder.Configuration.GetValue(
            "Authentication:RequireHttpsMetadata",
            !builder.Environment.IsDevelopment());
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role,
            ValidateIssuer = true,
            ValidIssuer = authAuthority,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        if (!string.IsNullOrWhiteSpace(authBackchannelAuthority))
        {
            options.MetadataAddress = $"{authBackchannelAuthority.TrimEnd('/')}/.well-known/openid-configuration";
            options.BackchannelHttpHandler = new KeycloakBackchannelHandler(
                new Uri(authAuthority),
                new Uri(authBackchannelAuthority));
        }

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                    return Task.CompletedTask;

                var realmAccess = context.Principal.FindFirst("realm_access")?.Value;
                if (string.IsNullOrWhiteSpace(realmAccess))
                    return Task.CompletedTask;

                using var document = JsonDocument.Parse(realmAccess);
                if (!document.RootElement.TryGetProperty("roles", out var roles))
                    return Task.CompletedTask;

                foreach (var role in roles.EnumerateArray())
                {
                    var value = role.GetString();
                    if (!string.IsNullOrWhiteSpace(value) && !identity.HasClaim(ClaimTypes.Role, value))
                        identity.AddClaim(new Claim(ClaimTypes.Role, value));
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

// The React development client runs separately from the API. Keep the origin
// allow-list configuration driven so production can provide its own values.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? ["http://localhost:5173"];

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API", Version = "v1" });
});

// Add Rate Limiting
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimitOptions"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

var app = builder.Build();

// Containerized development can initialize a fresh SQL Server volume without
// baking database credentials or migration tooling into the runtime image.
if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Use Rate Limiting
app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
    });
}

if (!builder.Configuration.GetValue<bool>("Http:DisableHttpsRedirection"))
    app.UseHttpsRedirection();
app.UseCors("Frontend");
var uploadRoot = Path.Combine(app.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadRoot),
    RequestPath = "/uploads"
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

sealed class KeycloakBackchannelHandler : HttpClientHandler
{
    private readonly Uri _publicAuthority;
    private readonly Uri _backchannelAuthority;

    public KeycloakBackchannelHandler(Uri publicAuthority, Uri backchannelAuthority)
    {
        _publicAuthority = publicAuthority;
        _backchannelAuthority = backchannelAuthority;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null &&
            request.RequestUri.AbsoluteUri.StartsWith(_publicAuthority.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
        {
            request.RequestUri = new Uri(
                _backchannelAuthority,
                request.RequestUri.AbsoluteUri[_publicAuthority.AbsoluteUri.Length..]);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
