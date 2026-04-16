# Agent Overview - DevBoard Project (.NET + PostgreSQL + Vue)

## Stack
- **Backend:** .NET 8 + ASP.NET Core Web API
- **Database:** PostgreSQL 16 + EF Core Migrations (ve egitim icin SQL migration klasoru)
- **Frontend:** Vue 3 + Composition API + Pinia + Vue Router + TypeScript
- **Build:** dotnet CLI (backend), Vite (frontend)
- **Auth:** JWT Bearer Authentication

## Project Structure
```text
devboard/
|- backend/
|  |- DevBoard.Api/
|  |  |- Controllers/        # REST endpoint'leri
|  |  |- Data/               # DbContext + EF configuration
|  |  |- Domain/             # Entity + enum
|  |  |- DTOs/               # Request/Response modelleri
|  |  |- Services/           # Is kurallari + token uretimi
|  |  |- Middleware/         # Global error middleware
|  |  |- Program.cs
|  |  `- DevBoard.Api.csproj
|- db/
|  `- migrations/            # SQL referans migrationlari
|- frontend/
|  |- src/
|  |  |- api/
|  |  |- components/
|  |  |- composables/
|  |  |- layouts/
|  |  |- router/
|  |  |- stores/
|  |  |- types/
|  |  `- views/
|  |- package.json
|  `- vite.config.ts
`- .agents/
```

## Developer Background Shift (MERN -> .NET/Vue)
- MongoDB -> PostgreSQL: collection dusuncesi yerine iliski ve JOIN odakli dusun.
- Express middleware -> ASP.NET middleware + filters.
- Mongoose schema -> EF Core entity + Fluent API + migrations.
- React hooks -> Vue composables + Pinia store.

## Agent Files Index
| File | Purpose |
|------|---------|
| `01-database.md` | PostgreSQL schema kurallari + migration prensipleri |
| `02-backend.md` | .NET Web API coding standards |
| `03-frontend.md` | Vue 3 / Pinia / Router standards |
| `04-api-contract.md` | REST API sozlesmesi ve DTO formatlari |
| `05-auth.md` | JWT auth flow (.NET tarafi) |
| `06-error-handling.md` | Tum katmanlarda hata yonetimi |
