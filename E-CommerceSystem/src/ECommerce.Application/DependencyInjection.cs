using ECommerce.Application;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.ValueObjects;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        // Configure Value Object mappings
        config.ForType<Price, decimal>()
            .MapWith(src => src.Value);
        config.ForType<decimal, Price>()
            .MapWith(src => new Price(src));

        config.ForType<Email, string>()
            .MapWith(src => src.Value);
        config.ForType<string, Email>()
            .MapWith(src => new Email(src));

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<IOrderStockService, OrderStockService>();

        return services;
    }
}
