FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

COPY ECommerce.sln ./
COPY src/ECommerce.Domain/ECommerce.Domain.csproj src/ECommerce.Domain/
COPY src/ECommerce.Application/ECommerce.Application.csproj src/ECommerce.Application/
COPY src/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj src/ECommerce.Infrastructure/
COPY src/ECommerce.Api/ECommerce.Api.csproj src/ECommerce.Api/
RUN dotnet restore ECommerce.sln

FROM restore AS publish
COPY . .
RUN dotnet publish src/ECommerce.Api/ECommerce.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0
EXPOSE 8080
COPY --from=publish /app/publish .
RUN mkdir -p /app/uploads && chown -R ${APP_UID}:${APP_UID} /app/uploads
USER $APP_UID
ENTRYPOINT ["dotnet", "ECommerce.Api.dll"]
