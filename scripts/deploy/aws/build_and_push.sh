#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../../../" && pwd)"
ENV_FILE="${1:-$ROOT_DIR/deploy/aws/.env.aws}"
OUT_FILE="$ROOT_DIR/deploy/aws/.images.generated.env"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Env file not found: $ENV_FILE"
  echo "Copy deploy/aws/.env.aws.example -> deploy/aws/.env.aws and fill it first."
  exit 1
fi

# shellcheck source=/dev/null
source "$ENV_FILE"

: "${AWS_REGION:?AWS_REGION is required}"
: "${AWS_ACCOUNT_ID:?AWS_ACCOUNT_ID is required}"
: "${ECR_BACKEND_REPO:?ECR_BACKEND_REPO is required}"
: "${ECR_FRONTEND_REPO:?ECR_FRONTEND_REPO is required}"

ECR_REGISTRY="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
TAG="$(git -C "$ROOT_DIR" rev-parse --short HEAD 2>/dev/null || date +%Y%m%d%H%M%S)"
BACKEND_IMAGE="${ECR_REGISTRY}/${ECR_BACKEND_REPO}:${TAG}"
FRONTEND_IMAGE="${ECR_REGISTRY}/${ECR_FRONTEND_REPO}:${TAG}"

echo "[1/5] Ensuring ECR repos exist..."
aws ecr describe-repositories --region "$AWS_REGION" --repository-names "$ECR_BACKEND_REPO" >/dev/null 2>&1 || \
  aws ecr create-repository --region "$AWS_REGION" --repository-name "$ECR_BACKEND_REPO" >/dev/null

aws ecr describe-repositories --region "$AWS_REGION" --repository-names "$ECR_FRONTEND_REPO" >/dev/null 2>&1 || \
  aws ecr create-repository --region "$AWS_REGION" --repository-name "$ECR_FRONTEND_REPO" >/dev/null

echo "[2/5] Logging into ECR..."
aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$ECR_REGISTRY"

echo "[3/5] Building backend image: $BACKEND_IMAGE"
docker build -f "$ROOT_DIR/backend/DevBoard.Api/Dockerfile" -t "$BACKEND_IMAGE" "$ROOT_DIR"

echo "[4/5] Building frontend image: $FRONTEND_IMAGE"
docker build -f "$ROOT_DIR/frontend/Dockerfile" --build-arg VITE_API_URL="${VITE_API_URL:-/api/v1}" -t "$FRONTEND_IMAGE" "$ROOT_DIR/frontend"

echo "[5/5] Pushing images..."
docker push "$BACKEND_IMAGE"
docker push "$FRONTEND_IMAGE"

cat > "$OUT_FILE" <<EOV
BACKEND_IMAGE=$BACKEND_IMAGE
FRONTEND_IMAGE=$FRONTEND_IMAGE
EOV

echo "Generated: $OUT_FILE"
echo "BACKEND_IMAGE=$BACKEND_IMAGE"
echo "FRONTEND_IMAGE=$FRONTEND_IMAGE"
