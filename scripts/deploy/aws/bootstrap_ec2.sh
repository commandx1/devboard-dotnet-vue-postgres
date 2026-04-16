#!/usr/bin/env bash
set -euo pipefail

# Amazon Linux 2023
if [[ -f /etc/system-release ]] && grep -qi "Amazon Linux" /etc/system-release; then
  sudo dnf update -y
  sudo dnf install -y docker git awscli
  sudo systemctl enable --now docker
  sudo usermod -aG docker "$USER"
  sudo mkdir -p /usr/local/lib/docker/cli-plugins
  sudo curl -SL "https://github.com/docker/compose/releases/download/v2.27.0/docker-compose-linux-$(uname -m)" -o /usr/local/lib/docker/cli-plugins/docker-compose
  sudo chmod +x /usr/local/lib/docker/cli-plugins/docker-compose
  echo "Bootstrap completed. Re-login to apply docker group permissions."
  exit 0
fi

# Ubuntu / Debian
if command -v apt-get >/dev/null 2>&1; then
  sudo apt-get update
  sudo apt-get install -y ca-certificates curl gnupg awscli
  sudo install -m 0755 -d /etc/apt/keyrings
  curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
  sudo chmod a+r /etc/apt/keyrings/docker.gpg
  echo \
    "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
    $(. /etc/os-release && echo \"$VERSION_CODENAME\") stable" | sudo tee /etc/apt/sources.list.d/docker.list >/dev/null
  sudo apt-get update
  sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
  sudo usermod -aG docker "$USER"
  sudo systemctl enable --now docker
  echo "Bootstrap completed. Re-login to apply docker group permissions."
  exit 0
fi

echo "Unsupported distro. Install docker + docker compose + awscli manually."
