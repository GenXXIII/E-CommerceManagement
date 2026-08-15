# NEXRIG Docker stack

The Compose stack builds two project images and runs six containers:

- `ecommerce-web:dev` — React production bundle served by Nginx.
- `ecommerce-api:dev` — ASP.NET Core 9 API.
- SQL Server 2022 Developer.
- Redis 7.4 with append-only persistence.
- Keycloak 26.7 for OpenID Connect login, logout, roles, and password reset.
- Mailpit for receiving local password-reset emails.

1. Copy the root environment example:

   ```powershell
   Copy-Item ..\.env.example ..\.env
   ```

2. Replace `SQL_SA_PASSWORD` in `.env` with a strong local password. SQL Server
   rejects short passwords such as `1234`. username `local,1433` 

3. Build the frontend and API images, then start the complete stack:

   ```powershell
   docker compose up --build -d
   ```

4. Open the application:

   - Frontend: `http://localhost:5173`
   - API: `http://localhost:5267`
   - Swagger: `http://localhost:5267/swagger`
   - API health: `http://localhost:5267/health`
   - Keycloak admin: `http://localhost:8080/admin`
   - Local password-reset inbox: `http://localhost:8025`

Use `user / 1234` for the customer account or `admin / 1234` for the operations
account. The Keycloak admin credentials come from `KEYCLOAK_ADMIN_USERNAME` and
`KEYCLOAK_ADMIN_PASSWORD` in the root `.env` file.

New customers can select **Create account** in the storefront or open
`http://localhost:5173/register`. Keycloak assigns the customer role
automatically after registration.

The API waits for healthy SQL Server and Redis containers, then automatically
applies the checked-in EF Core migrations to a new database volume. Nginx serves
the SPA and proxies browser requests under `/api` to the API container.

Useful commands:

```powershell
# Show container state
docker compose --env-file ..\.env ps

# Follow all logs
docker compose --env-file ..\.env logs -f

# Rebuild only application images
docker compose --env-file ..\.env build frontend api

# Stop without deleting database/cache volumes
docker compose --env-file ..\.env down
```

The named volumes keep SQL Server and Redis data across container restarts. Use
`docker compose down -v` only when you intentionally want to delete local SQL
Server and Redis data.

The local stack uses Keycloak development mode and Mailpit. Before production,
run Keycloak in production mode with TLS, a production database, real SMTP,
strict production origins/redirect URIs, and secrets supplied by your deployment
platform.
