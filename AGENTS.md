# AGENTS.md

## Repo layout (separate .NET solutions)
This repo contains multiple independent solutions (no single root solution). Each folder is its own solution/project set:

- `ConsoleApp1/` - Simple .NET 8 console app (`ConsoleApp1.sln`, project `ConsoleApp1/ConsoleApp1.csproj`)
- `WebApi projekt/` - Minimal .NET 10 ASP.NET Core Web API (`WebApi projekt.sln`, project `WebApi projekt/WebApi projekt.csproj`) - uses controllers + OpenAPI
- `WebApplication1/` - Layered .NET 8 Web API (`WebApplication1.sln`) - projects: `WebApplication1` (API), `Zoo.service`, `Zoo.repository`, `Zoo.model`, `Zoo.common`. Uses EF Core + Npgsql, JWT auth, Swagger in dev
- `HIreCraft/` - Layered .NET 10 Web API (`HireCraft.sln`) - projects: `HireCraft.Api`, `HireCraft.Service`, `HireCraft.DAL`, `HireCraft.Common`. Uses EF Core + Npgsql (schema `g02`), Dapper, JWT auth, custom middleware

## Build/run (dotnet CLI)
Run from solution/project directory. Examples:

```bash
# Console app
cd ConsoleApp1
dotnet run --project ConsoleApp1/ConsoleApp1.csproj

# Minimal Web API (.NET 10)
cd "WebApi projekt"
dotnet run --project "WebApi projekt/WebApi projekt.csproj"

# WebApplication1 (.NET 8) - Swagger enabled in dev
cd WebApplication1
dotnet run --project WebApplication1/WebApplication1.csproj

# HireCraft (.NET 10)
cd HIreCraft
dotnet run --project HireCraft.Api/HireCraft.Api.csproj
```

Build: `dotnet build <sln-or-csproj>`. Restore happens implicitly on build/run.

## Key quirks / gotchas
- **Multiple targets**: `ConsoleApp1` and `WebApplication1` target `net8.0`; `WebApi projekt` and `HIreCraft` target `net10.0`. Use correct SDK/runtime.
- **HireCraft**: 
  - DB schema `g02` used (`__EFMigrationsHistory` in schema `g02`, connection to `praksa2026-listopad-g02` on `dbs.edukacija.local:5432`).
  - Auth: JWT via `JwtSettings` (Issuer/Audience in appsettings; Key from User Secrets). `ExceptionMiddleware` registered. Decimal separator forced to InvariantCulture.
  - DI registers many repos/services (Auth, Role, User, City, County, Handyman, JobType, JobItem, Favorite, Booking, Review, TransactionRunner). Also registers `IDbConnection` as NpgsqlConnection.
- **WebApplication1**:
  - Requires `ConnectionStrings:ZooDatabase`. JWT config under `Jwt` section validated strictly (Base64 key must decode to >=32 bytes; all fields required; clock skew zero). Uses `NpgsqlDataSource` singleton + EF Core `ZooDbContext`. Exception handling via `ExceptionHandlingMiddleware`. Swagger only in Development.
- **WebApi projekt**: Very minimal .NET 10 API template (controllers + OpenAPI only).
- **Secrets/connection strings**: HireCraft and WebApplication1 have `UserSecretsId` in csproj; `WebApi projekt` has none. Check user-secrets if running APIs that need secrets.
- **.gitignore** is standard Visual Studio/.NET ignore.

## Notes
- No root README/AGENTS/opencode config found. Each solution is standalone.
- When working in a specific solution, prefer its project files and appsettings for accurate config.
- `nesto novo.txt` is empty.