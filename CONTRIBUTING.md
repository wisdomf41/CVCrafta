<!-- CVCrafta contributor guidelines -->

# Contributing to CVCrafta

Thank you for your interest in contributing to **CVCrafta**.

CVCrafta is a full-stack ASP.NET Core and React project being developed with an emphasis on maintainability, secure development practices, automated testing, reproducible local environments, and meaningful open-source collaboration.

Contributions are welcome through issues, discussions, documentation improvements, bug fixes, tests, and feature pull requests.

---

## Code of Conduct

Contributors are expected to communicate respectfully and constructively.

A dedicated `CODE_OF_CONDUCT.md` will be maintained as part of the repository's open-source governance.

---

## Project Stack

Before contributing, it is helpful to be familiar with some of the technologies used in CVCrafta:

* C#
* ASP.NET Core 10
* Entity Framework Core
* ASP.NET Core Identity
* JWT authentication
* SQL Server
* Redis
* React 19
* Vite
* Tailwind CSS
* TanStack Query
* Axios
* Vitest
* React Testing Library
* Docker
* Docker Compose
* Git and GitHub

Future production-focused work will also introduce technologies such as OpenTelemetry, Prometheus, Grafana, Azure Application Insights, Terraform, Bicep, and additional CI/CD automation.

---

## Repository Structure

The main areas of the repository are:

```text
CVCrafta/
│
├── .github/
│
├── docs/
│
├── resumeapp.client/
│
├── ResumeApp.Server/
│
├── ResumeApp.Server.Tests/
│
├── docker-compose.yml
├── ResumeApp.slnx
├── README.md
└── CONTRIBUTING.md
```

CVCrafta is the public product name.

Some internal project names and namespaces still use `ResumeApp`. Contributors should not rename these internal projects or namespaces unless the change is part of an explicitly approved refactoring task.

---

## Before You Start

Before beginning significant work:

1. Check existing issues and pull requests.
2. Confirm that another contributor is not already working on the same change.
3. For larger features or architectural changes, open an issue or discussion before implementation.
4. Keep pull requests focused on one meaningful change.

Avoid combining unrelated refactoring, formatting changes, dependency upgrades, and feature work in the same pull request.

---

## Development Prerequisites

Install the following:

* .NET 10 SDK
* Visual Studio with ASP.NET and web development workloads
* Node.js
* Docker Desktop
* Git

Visual Studio is the recommended primary development environment for CVCrafta.

---

## Fork and Clone

Fork the repository using GitHub, then clone your fork:

```powershell
git clone <your-fork-url>
cd ResumeApp
```

Add the original CVCrafta repository as an upstream remote:

```powershell
git remote add upstream <official-repository-url>
```

Verify the remotes:

```powershell
git remote -v
```

---

## Create a Branch

Do not make feature changes directly on `main`.

Create a branch for your work:

```powershell
git switch -c feature/short-description
```

Examples:

```text
feature/resume-export
feature/opentelemetry
fix/email-confirmation
fix/resume-validation
docs/docker-setup
test/authentication-edge-cases
```

Use short, descriptive branch names.

---

## Environment Setup

Sensitive values must never be committed to source control.

Create a local `.env` file from the provided template:

```powershell
Copy-Item .env.example .env
```

Provide your own local values for the variables contained in `.env.example`.

Example:

```text
SQL_SA_PASSWORD=<strong-local-password>
JWT_KEY=<long-random-jwt-key>
```

Do not commit `.env`.

---

## .NET User Secrets

When running the backend outside Docker, sensitive development settings may be stored with .NET User Secrets.

From `ResumeApp.Server`:

```powershell
dotnet user-secrets set "Jwt:Key" "<your-jwt-key>"
```

A development database connection string can also be stored using:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```

Email-delivery credentials must also remain outside tracked configuration files.

---

## Running with Visual Studio

Open:

```text
ResumeApp.slnx
```

in Visual Studio.

For the standard containerized development environment, use the **Docker Compose** launch profile.

The Docker environment includes:

* ASP.NET Core API
* SQL Server
* Redis

The infrastructure services include health checks and are configured through Docker Compose.

---

## Running the Frontend

From the repository root:

```powershell
cd resumeapp.client
npm install
npm run dev
```

---

## Running the Backend

If running outside Docker, make sure the required SQL Server, Redis, and local secrets are available.

Then:

```powershell
cd ResumeApp.Server
dotnet restore
dotnet run
```

---

## Database Changes

CVCrafta uses Entity Framework Core.

When your contribution changes the database model, an appropriate EF Core migration may be required.

Do not:

* delete existing migrations without discussion
* rewrite migration history casually
* manually alter shared production-oriented schemas without migration support
* commit connection strings or database credentials

Database migrations should represent intentional application-model changes.

---

## Coding Guidelines

Keep changes consistent with the existing project architecture.

### Backend

Prefer:

* asynchronous APIs for I/O operations
* dependency injection
* DTOs for API input/output boundaries
* clear validation
* authorization checks
* user ownership enforcement
* service-layer business logic
* nullable reference type correctness
* focused methods
* meaningful names

Avoid placing substantial business logic directly inside controllers.

### Frontend

Prefer:

* reusable React components
* clear separation of API/data-fetching behavior
* TanStack Query for server-state management where appropriate
* accessible UI behavior
* responsive layouts
* predictable authentication state handling
* components that are straightforward to test

Avoid duplicating existing API or authentication logic.

---

## Security Requirements

Security-sensitive changes require additional care.

Never commit:

* passwords
* API keys
* JWT signing keys
* connection-string credentials
* SMTP passwords
* private certificates
* production secrets
* populated `.env` files

Do not weaken authentication or authorization behavior to make a feature easier to implement.

Changes affecting the following areas should include appropriate tests:

* authentication
* authorization
* resource ownership
* registration
* login
* email confirmation
* password handling
* JWT behavior
* sensitive configuration

Security vulnerabilities should be reported according to the repository's `SECURITY.md` rather than disclosed through a public issue.

---

## Backend Tests

Run the complete .NET test suite from the repository root:

```powershell
dotnet test
```

All existing tests should pass before opening a pull request.

Changes to backend behavior should include new or updated tests where appropriate.

---

## Frontend Tests

Run:

```powershell
npm --prefix .\resumeapp.client run test:run
```

Changes to frontend behavior should include tests where practical.

---

## Frontend Linting

Run:

```powershell
npm --prefix .\resumeapp.client run lint
```

Resolve relevant lint errors introduced by your changes before submitting a pull request.

---

## Build Verification

Verify the backend builds successfully:

```powershell
dotnet build
```

Verify the frontend production build:

```powershell
npm --prefix .\resumeapp.client run build
```

Contributions should not knowingly leave the repository in a broken build state.

---

## Keep Your Branch Updated

Before opening or updating a pull request:

```powershell
git fetch upstream
git switch main
git pull upstream main
git switch <your-branch>
git rebase main
```

If your team workflow uses merge rather than rebase, follow the repository's current maintainer guidance.

Resolve conflicts carefully and rerun the relevant tests after resolving them.

---

## Commit Messages

Use concise commit messages that describe the change.

Examples:

```text
Add resume export endpoint
Fix email confirmation state refresh
Add Redis cache integration tests
Improve contributor setup documentation
Instrument API requests with OpenTelemetry
```

Avoid vague commit messages such as:

```text
update
changes
fix stuff
work
final
```

---

## Pull Requests

A pull request should clearly explain:

* what changed
* why the change was needed
* how it was tested
* whether database migrations were added
* whether configuration changed
* whether documentation should be updated

Keep pull requests reasonably focused.

Large unrelated changes are harder to review and more likely to introduce regressions.

---

## Pull Request Checklist

Before submitting a pull request, confirm:

* [ ] The project builds successfully.
* [ ] Existing backend tests pass.
* [ ] Existing frontend tests pass.
* [ ] New behavior has appropriate tests where practical.
* [ ] Frontend linting passes for affected code.
* [ ] No secrets were added.
* [ ] `.env` was not committed.
* [ ] Authentication and authorization behavior remains secure.
* [ ] Database changes include appropriate migrations.
* [ ] Documentation was updated when necessary.
* [ ] The pull request contains only relevant changes.

---

## Documentation Contributions

Documentation improvements are welcome.

Project documentation lives primarily in:

```text
README.md
CONTRIBUTING.md
docs/
```

Technical documentation should describe behavior that actually exists.

Planned capabilities should be clearly identified as planned rather than presented as already implemented.

---

## Observability Contributions

CVCrafta's production roadmap includes:

* OpenTelemetry
* Prometheus
* Grafana
* Azure Application Insights
* structured production logging
* distributed tracing
* application metrics
* health monitoring

Observability changes should preferably follow vendor-neutral OpenTelemetry instrumentation where appropriate and avoid unnecessarily coupling application code to a single monitoring platform.

---

## Infrastructure Contributions

Planned infrastructure work includes:

* Microsoft Azure
* Terraform
* Bicep
* GitHub Actions
* CI/CD
* container deployment
* Kubernetes where appropriate

Infrastructure changes should remain reproducible and should never contain production credentials.

---

## Reporting Bugs

When reporting a bug, provide enough information to reproduce it where possible:

* affected feature
* expected behavior
* actual behavior
* reproduction steps
* operating environment
* relevant logs or error messages

Remove passwords, access tokens, email credentials, personal information, and other secrets before sharing logs.

---

## Feature Requests

Feature requests should explain:

* the problem being solved
* the proposed behavior
* why the feature belongs in CVCrafta
* any major implementation considerations

A problem-focused feature request is usually more useful than prescribing a specific implementation.

---

## Scope of Contributions

Good contributions may include:

* bug fixes
* backend features
* frontend improvements
* API improvements
* automated tests
* documentation
* accessibility improvements
* performance improvements
* observability
* CI/CD
* security hardening
* infrastructure automation

Changes that significantly alter the application's architecture should be discussed before implementation.

---

## Review Process

Maintainers may request changes before a pull request is merged.

Review may consider:

* correctness
* security
* maintainability
* tests
* architecture
* documentation
* backward compatibility
* scope
* consistency with project goals

A pull request may be declined if it introduces unnecessary complexity, duplicates existing functionality, weakens security, or falls outside the current project direction.

---

## Thank You

Every useful contribution helps make CVCrafta a stronger project.

Whether you contribute code, tests, documentation, bug reports, infrastructure improvements, or thoughtful technical discussion, your participation is appreciated.
