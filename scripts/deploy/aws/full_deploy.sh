#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/../../../" && pwd)"
ENV_FILE="${1:-$ROOT_DIR/deploy/aws/.env.aws}"

"$SCRIPT_DIR/build_and_push.sh" "$ENV_FILE"
"$SCRIPT_DIR/apply_sql_migrations.sh" "$ENV_FILE"
"$SCRIPT_DIR/deploy_to_ec2.sh" "$ENV_FILE" "$ROOT_DIR/deploy/aws/.images.generated.env"

echo "Full deploy finished successfully."
