# Contributing Guide

Thanks for contributing.

## Development Setup

1. Start local DB:
```bash
docker compose up -d postgres
```
2. Run backend:
```bash
cd backend/DevBoard.Api
dotnet restore
dotnet run
```
3. Run frontend:
```bash
cd frontend
npm install
npm run dev
```

## Pull Request Checklist

- Keep changes focused and minimal
- Update docs for behavior/config changes
- Ensure local build passes:
  - `dotnet build backend/DevBoard.Api`
  - `npm --prefix frontend run build`
- Do not commit secrets or `.env` files

## Commit Style

Use clear, imperative messages, for example:
- `feat(auth): add register endpoint validation`
- `fix(frontend): handle missing user error on login`
- `docs: add aws deployment runbook`
