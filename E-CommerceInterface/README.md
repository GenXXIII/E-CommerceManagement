# Cartana frontend

React + TypeScript UI for the ASP.NET Core e-commerce backend.

## Development

1. Start SQL Server and Redis for the backend. A Docker Compose setup is in
   `../infrastructure`; follow its README to configure the local SQL password.
2. Run the API:

   ```powershell
   cd ..\E-CommerceSystem
   dotnet run --project src\ECommerce.Api\ECommerce.Api.csproj
   ```

3. Run the frontend:

   ```powershell
   npm install
   npm run dev
   ```

Vite proxies `/api` to `http://localhost:5267` in development. Set
`VITE_API_BASE_URL` for other environments.

## Docker

The repository-level Compose stack builds this frontend into `cartana-web:dev`
and serves the production bundle with Nginx. Nginx handles React Router fallback
and proxies `/api` to the `cartana-api:dev` container.

See [the infrastructure guide](../infrastructure/README.md) for the one-command
full-stack startup.

## Temporary development login

- Customer: `user` / `1234`
- Administrator: `admin` / `1234`

The API exposes the fixed login only in the Development environment. It creates
or reuses one persisted demo customer profile so cart, wishlist, address, order,
and payment calls can use the backend's current customer-ID contracts.

This is not production authentication or authorization. The UI consumes only
the `useAuth()` boundary so a later Keycloak adapter can replace the temporary
provider without rewriting feature pages.

## Quality checks

```powershell
npm run lint
npm run test
npm run build
```

Runtime business data is never mocked. Screens use implemented API endpoints or
show an explicit loading, error, empty, or unavailable state.
