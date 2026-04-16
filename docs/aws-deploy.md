# AWS Deployment Guide (EC2 + ECR + RDS)

This guide deploys DevBoard as:
- Backend container on EC2 (from ECR)
- Frontend container on EC2 (from ECR)
- PostgreSQL on Amazon RDS (managed, separate from EC2)

## 1) One-time AWS setup

1. Create an RDS PostgreSQL instance (private/public per your network policy).
2. Create an EC2 instance for running Docker containers.
3. Open security groups:
   - EC2 inbound: `80` (and `443` if you add TLS proxy)
   - RDS inbound: `5432` from EC2 security group
4. Install Docker + Compose + AWS CLI on EC2:

```bash
bash scripts/deploy/aws/bootstrap_ec2.sh
```

## 2) Local deploy configuration

Create your deploy env:

```bash
cp deploy/aws/.env.aws.example deploy/aws/.env.aws
```

Fill values in `deploy/aws/.env.aws`:
- AWS account/region
- ECR repo names
- EC2 host/user/key path
- RDS host/user/password/db
- JWT secret
- CORS allowed origins

## 3) Full deploy (single command)

```bash
bash scripts/deploy/aws/full_deploy.sh deploy/aws/.env.aws
```

This command does:
1. Build backend/frontend Docker images
2. Push images to ECR
3. Apply SQL migrations to RDS
4. Deploy latest images to EC2 via Docker Compose

## 4) Partial commands (when needed)

Build & push only:

```bash
bash scripts/deploy/aws/build_and_push.sh deploy/aws/.env.aws
```

Run migrations only:

```bash
bash scripts/deploy/aws/apply_sql_migrations.sh deploy/aws/.env.aws
```

Deploy to EC2 only:

```bash
bash scripts/deploy/aws/deploy_to_ec2.sh deploy/aws/.env.aws deploy/aws/.images.generated.env
```

## 5) Verify deployment

SSH into EC2 and run:

```bash
cd /opt/devboard
docker compose --env-file .env.aws ps
docker compose --env-file .env.aws logs -f backend
```

Health endpoint:

```bash
curl http://<EC2_PUBLIC_IP_OR_DOMAIN>/api/v1/health
```

## Notes

- In production, keep RDS separate from app containers.
- Do not commit `deploy/aws/.env.aws`.
- If you use a domain + HTTPS, place an ALB or reverse proxy in front of EC2.
