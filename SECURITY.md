# Security Policy

## Overview

Security is an important part of **CVCrafta**.

CVCrafta includes authentication, authorization, email confirmation, JWT-based access control, SQL Server persistence, Redis caching, Docker-based infrastructure, and external configuration for sensitive values.

If you discover a security vulnerability, please report it responsibly rather than disclosing it through a public issue.

---

## Supported Versions

CVCrafta is currently under active development and has not yet reached a formal production release.

Security fixes are applied to the actively maintained version of the project.

| Version                     | Supported |
| --------------------------- | --------- |
| Current development version | Yes       |
| Older development snapshots | No        |

This policy may be updated when versioned public releases are introduced.

---

## Reporting a Vulnerability

Do **not** create a public GitHub issue for a suspected security vulnerability.

Examples include:

* authentication bypasses
* authorization failures
* user ownership bypasses
* exposed secrets or credentials
* JWT vulnerabilities
* account takeover risks
* insecure email-confirmation behavior
* SQL injection
* cross-site scripting
* sensitive information exposure
* privilege escalation
* insecure configuration
* container or infrastructure security issues

Use GitHub's private vulnerability reporting mechanism when it becomes available for the public repository.

Until the repository's private reporting channel is finalized, avoid publicly disclosing exploit details.

---

## What to Include

A useful vulnerability report should include:

* a clear description of the issue
* the affected area
* steps to reproduce the issue
* expected behavior
* actual behavior
* potential security impact
* relevant logs or screenshots
* environment information where relevant
* suggested mitigation, if known

Do not include unrelated personal information or secrets in the report.

---

## Sensitive Information

Never include the following in bug reports, pull requests, discussions, screenshots, or logs:

* passwords
* JWT signing keys
* access tokens
* refresh tokens
* SMTP passwords
* API keys
* database passwords
* production connection strings
* private certificates
* populated `.env` files
* private user information

Before sharing logs, remove or redact sensitive values.

---

## Secret Management

CVCrafta is designed so sensitive development and production values remain outside tracked source files.

Local configuration may use:

* `.env`
* .NET User Secrets
* environment variables
* deployment-platform secret stores

The repository includes `.env.example` only as a safe configuration template.

The actual `.env` file must never be committed.

---

## Authentication Security

CVCrafta uses ASP.NET Core Identity and JWT bearer authentication.

Changes affecting authentication should preserve:

* password hashing through ASP.NET Core Identity
* secure JWT signing
* token validation
* authenticated endpoint protection
* account confirmation requirements
* correct user identity propagation

Authentication protections should not be bypassed merely to simplify development or testing.

---

## Authorization and Ownership

Protected resources must enforce appropriate authorization.

Where data belongs to an individual user, API operations must verify that the authenticated user is authorized to access or modify that resource.

Contributions should not weaken resource ownership checks.

Security-sensitive authorization behavior should have automated test coverage.

---

## Email Confirmation

CVCrafta includes an email-confirmation workflow.

Changes to this flow should be reviewed carefully for:

* token validation
* token expiration behavior
* replay behavior
* confirmation-state synchronization
* redirect handling
* automatic-login behavior
* information disclosure

Email-confirmation URLs and tokens should be treated as sensitive data.

---

## JWT Security

JWT signing keys must be:

* sufficiently long
* randomly generated
* stored outside source control
* different between appropriate environments
* rotated when necessary

JWT signing keys must never be committed to the repository.

---

## Database Security

CVCrafta uses SQL Server through Entity Framework Core.

Database-related contributions should avoid:

* hard-coded credentials
* unsafe raw SQL
* unnecessary exposure of database details
* bypassing Entity Framework parameterization
* destructive migration changes without review

Database schema changes should be applied through intentional migrations.

---

## Redis Security

Redis is used as part of the CVCrafta application infrastructure.

Redis should not be exposed publicly without appropriate network protections and authentication controls.

Production Redis configuration should follow the security controls provided by the selected hosting environment.

---

## Docker Security

Docker images and Compose configuration must not contain embedded production secrets.

Sensitive configuration should be injected at runtime.

Contributors should avoid:

* committing passwords into Dockerfiles
* embedding JWT keys into images
* storing secrets in tracked Compose files
* exposing unnecessary container ports
* running unnecessary privileged containers

---

## Dependency Security

Dependencies should be maintained responsibly.

When adding a dependency:

* verify that it is actively maintained
* use trusted package sources
* avoid unnecessary dependencies
* review known security concerns
* pin or constrain versions appropriately where practical

Security-related dependency updates should be tested before merging.

---

## Frontend Security

Frontend changes should avoid introducing:

* unsafe HTML rendering
* token exposure
* sensitive data in browser logs
* insecure local storage practices beyond existing application design
* untrusted script execution
* accidental exposure of backend secrets

Secrets must never be placed in frontend source code because browser-delivered code is visible to users.

---

## Logging

Logs must not expose:

* passwords
* JWT signing keys
* access tokens
* email-confirmation tokens
* SMTP credentials
* database passwords
* other secret configuration

Production observability work should preserve this requirement.

---

## Observability Security

CVCrafta's planned observability stack includes:

* OpenTelemetry
* Prometheus
* Grafana
* Azure Application Insights

Telemetry must not unintentionally collect:

* passwords
* authentication tokens
* private credentials
* sensitive request bodies
* unnecessary personally identifiable information

Instrumentation should provide operational visibility without creating a new source of sensitive-data exposure.

---

## Infrastructure as Code

Planned infrastructure automation includes Terraform and Bicep.

Infrastructure code must not contain:

* production passwords
* private keys
* access tokens
* service credentials
* database secrets

Sensitive values should be provided using secure secret-management mechanisms.

---

## CI/CD Security

GitHub Actions and future deployment pipelines should follow least-privilege principles.

CI/CD workflows should:

* use repository secrets or approved secret stores
* avoid printing sensitive values
* limit token permissions
* avoid unnecessary privileged actions
* use trusted actions
* pin important actions where practical
* run automated security checks where appropriate

---

## Security Testing

Security-sensitive changes should include or update automated tests when practical.

Particular attention should be given to:

* authentication
* authorization
* ownership validation
* email confirmation
* validation failures
* token handling
* protected endpoints
* configuration behavior

Existing security-oriented tests should remain passing.

---

## Secret Scanning

The repository should be scanned for accidentally committed secrets as part of its open-source preparation and ongoing maintenance.

If a real secret is ever committed, removing it from the latest commit is not sufficient.

The secret should be considered compromised and rotated.

Repository history may also need remediation.

---

## Coordinated Disclosure

Please allow maintainers reasonable time to:

1. reproduce the vulnerability
2. assess its impact
3. prepare a fix
4. add regression tests
5. release or deploy the fix
6. communicate the issue appropriately

Public disclosure should occur only after a fix or mitigation is available when reasonably possible.

---

## Security Improvements

Security-focused contributions are welcome, including:

* authentication hardening
* authorization improvements
* additional security tests
* dependency security improvements
* secret-management improvements
* CI/CD security
* secure container configuration
* observability privacy safeguards
* infrastructure hardening

Large security-related architectural changes should be discussed before implementation.

---

## Responsible Research

Security research should not:

* access other users' private information
* intentionally damage data
* disrupt services
* perform destructive testing against production systems
* use discovered vulnerabilities for unauthorized access

Testing should be performed against environments and data you are authorized to use.

---

## Policy Updates

This security policy will evolve as CVCrafta moves from active development toward public release and production deployment.
