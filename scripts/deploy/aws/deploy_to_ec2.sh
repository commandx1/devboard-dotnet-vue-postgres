#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../../../" && pwd)"
ENV_FILE="${1:-$ROOT_DIR/deploy/aws/.env.aws}"
IMAGES_FILE="${2:-$ROOT_DIR/deploy/aws/.images.generated.env}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Env file not found: $ENV_FILE"
  exit 1
fi

if [[ ! -f "$IMAGES_FILE" ]]; then
  echo "Image env not found: $IMAGES_FILE"
  echo "Run scripts/deploy/aws/build_and_push.sh first."
  exit 1
fi

# shellcheck source=/dev/null
source "$ENV_FILE"
# shellcheck source=/dev/null
source "$IMAGES_FILE"

: "${AWS_REGION:?AWS_REGION is required}"
: "${AWS_ACCOUNT_ID:?AWS_ACCOUNT_ID is required}"
: "${EC2_HOST:?EC2_HOST is required}"
: "${EC2_USER:?EC2_USER is required}"
: "${EC2_SSH_KEY_PATH:?EC2_SSH_KEY_PATH is required}"
: "${EC2_DEPLOY_PATH:?EC2_DEPLOY_PATH is required}"
: "${BACKEND_IMAGE:?BACKEND_IMAGE is required}"
: "${FRONTEND_IMAGE:?FRONTEND_IMAGE is required}"

REMOTE_ENV_CONTENT=$(cat <<EOV
BACKEND_IMAGE=$BACKEND_IMAGE
FRONTEND_IMAGE=$FRONTEND_IMAGE
FRONTEND_PORT=${FRONTEND_PORT}
RDS_HOST=${RDS_HOST}
RDS_PORT=${RDS_PORT}
RDS_DB=${RDS_DB}
RDS_USER=${RDS_USER}
RDS_PASSWORD=${RDS_PASSWORD}
JWT_ISSUER=${JWT_ISSUER}
JWT_AUDIENCE=${JWT_AUDIENCE}
JWT_SECRET=${JWT_SECRET}
JWT_EXPIRATION_MINUTES=${JWT_EXPIRATION_MINUTES}
CORS_ALLOWED_ORIGINS=${CORS_ALLOWED_ORIGINS}
EOV
)

echo "Uploading compose + env to ${EC2_USER}@${EC2_HOST}:${EC2_DEPLOY_PATH}"
ssh -i "$EC2_SSH_KEY_PATH" "$EC2_USER@$EC2_HOST" "mkdir -p '$EC2_DEPLOY_PATH'"
scp -i "$EC2_SSH_KEY_PATH" "$ROOT_DIR/deploy/aws/docker-compose.aws.yml" "$EC2_USER@$EC2_HOST:$EC2_DEPLOY_PATH/docker-compose.yml"
ssh -i "$EC2_SSH_KEY_PATH" "$EC2_USER@$EC2_HOST" "cat > '$EC2_DEPLOY_PATH/.env.aws' <<'EOV'
$REMOTE_ENV_CONTENT
EOV"

echo "Deploying containers on EC2..."
ssh -i "$EC2_SSH_KEY_PATH" "$EC2_USER@$EC2_HOST" "
  set -euo pipefail
  aws ecr get-login-password --region '$AWS_REGION' | docker login --username AWS --password-stdin '$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com'
  cd '$EC2_DEPLOY_PATH'
  docker compose --env-file .env.aws pull
  docker compose --env-file .env.aws up -d --remove-orphans
  docker compose --env-file .env.aws ps
"

echo "EC2 deploy completed."
