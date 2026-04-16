#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../../../" && pwd)"
ENV_FILE="${1:-$ROOT_DIR/deploy/aws/.env.aws}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Env file not found: $ENV_FILE"
  exit 1
fi

# shellcheck source=/dev/null
source "$ENV_FILE"

: "${RDS_HOST:?RDS_HOST is required}"
: "${RDS_PORT:?RDS_PORT is required}"
: "${RDS_DB:?RDS_DB is required}"
: "${RDS_USER:?RDS_USER is required}"
: "${RDS_PASSWORD:?RDS_PASSWORD is required}"

MIGRATION_DIR="$ROOT_DIR/db/migrations"
if ! ls "$MIGRATION_DIR"/*.sql >/dev/null 2>&1; then
  echo "No SQL migrations found in $MIGRATION_DIR"
  exit 1
fi

echo "Applying SQL migrations to ${RDS_HOST}:${RDS_PORT}/${RDS_DB}..."
for file in "$MIGRATION_DIR"/*.sql; do
  base_name="$(basename "$file")"
  echo " -> $base_name"
  docker run --rm \
    -v "$MIGRATION_DIR:/migrations:ro" \
    -e PGPASSWORD="$RDS_PASSWORD" \
    postgres:16 \
    psql \
      "host=$RDS_HOST port=$RDS_PORT dbname=$RDS_DB user=$RDS_USER sslmode=require" \
      -v ON_ERROR_STOP=1 \
      -f "/migrations/$base_name"
done

echo "Migrations completed."
