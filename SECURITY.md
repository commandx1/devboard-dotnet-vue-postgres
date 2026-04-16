# Security Policy

## Supported Versions

This project is in active development. Security fixes are provided on the latest `main` branch.

## Reporting a Vulnerability

Please do **not** open a public issue for security vulnerabilities.

Use one of these options:
1. Open a private GitHub Security Advisory (preferred)
2. Contact the maintainer privately and include:
   - vulnerability summary
   - impact and affected components
   - reproduction steps / proof of concept

## Security Best Practices for This Project

- Keep real secrets outside of git (`.env`, cloud secret managers)
- Use long random JWT secrets (minimum 32 chars)
- Restrict CORS to trusted frontend origins only
- Keep RDS private to app network/security group
- Run least-privilege IAM permissions for deployment automation
- Rotate credentials regularly
