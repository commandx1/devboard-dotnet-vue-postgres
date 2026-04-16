# DevBoard (.NET + Vue + PostgreSQL)

A full-stack practice project built to learn and apply:
- ASP.NET Core Web API (.NET 8)
- Vue 3 + TypeScript + Pinia
- PostgreSQL (SQL-first schema + relational design)

## Tech Stack

- Backend: `ASP.NET Core 8`, `EF Core`, `JWT Bearer`
- Frontend: `Vue 3`, `Vite`, `Pinia`, `Vue Router`, `Axios`
- Database: `PostgreSQL 16`
- Containerization: `Docker`, `Docker Compose`
- Cloud deploy scripts: `AWS EC2 + ECR + RDS`

## Repository Structure

```text
backend/DevBoard.Api/      # .NET Web API
frontend/                  # Vue 3 application
db/migrations/             # SQL migration files
scripts/deploy/aws/        # Automated AWS deployment scripts
deploy/aws/                # AWS compose + env examples
docs/                      # Additional documentation
.agents/                   # Project conventions and learning requirements
```

## Local Development

### 1. Start PostgreSQL (Docker)

```bash
docker compose up -d postgres
```

Default local DB for this repo:
- host: `localhost`
- port: `5433`
- db: `devboard`
- user/password: `postgres/postgres`

### 2. Start Backend

```bash
cd backend/DevBoard.Api
dotnet restore
dotnet run
```

Backend health checks:
- `http://localhost:5000/health`
- `http://localhost:5000/api/v1/health`

### 3. Start Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend URL: `http://localhost:5173`

## Docker Production-Like Run (Local)

```bash
cp .env.prod.example .env.prod
# edit values

docker compose --env-file .env.prod -f docker-compose.prod.yml up -d --build
```

Check status:

```bash
docker compose --env-file .env.prod -f docker-compose.prod.yml ps
```

## AWS Deployment

This repository includes end-to-end scripts for:
- Building images
- Pushing to ECR
- Applying SQL migrations to RDS
- Deploying containers to EC2

Quick start:

```bash
cp deploy/aws/.env.aws.example deploy/aws/.env.aws
# fill required values

bash scripts/deploy/aws/full_deploy.sh deploy/aws/.env.aws
```

Detailed guide: [docs/aws-deploy.md](docs/aws-deploy.md)

## Security

- Never commit real credentials or secrets.
- Use `.env` files only locally; production secrets should come from secure secret stores.
- Review [SECURITY.md](SECURITY.md) before deploying to public environments.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).
