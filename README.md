<!-- CVCrafta public project README -->

# CVCrafta

**CVCrafta** is a full-stack resume management web application built with ASP.NET Core and React.

The project demonstrates production-oriented .NET development practices including JWT authentication, ASP.NET Core Identity, email confirmation, SQL Server persistence, Redis caching, Docker-based infrastructure, automated testing, API documentation, and a modern React frontend.

CVCrafta is currently being prepared for open-source release.

---

## Features

### Authentication and Account Management

* User registration and login
* JWT-based authentication
* ASP.NET Core Identity integration
* Email confirmation workflow
* Secure automatic login after email confirmation
* Confirmation-state detection when users return from another tab, window, browser session, or refresh the page
* Email confirmation resend handling
* Protected authenticated routes

### Resume Management

* Create resumes
* View public resumes
* View the authenticated user's resume
* Update resume information
* Delete resumes
* User ownership validation for protected operations
* Input validation using DTOs
* RESTful API endpoints

### Backend

* ASP.NET Core 10 Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT Bearer authentication
* Redis distributed caching
* MailKit email delivery
* Swagger / OpenAPI
* Dependency injection
* Asynchronous service operations
* Docker support

### Frontend

* React 19
* Vite
* React Router
* TanStack Query
* Axios
* Tailwind CSS
* React Hot Toast
* Authentication-aware navigation
* Responsive user interface
* Automated component and application tests

### Infrastructure

* Docker Compose
* SQL Server 2022
* Redis 7
* Persistent SQL Server Docker volume
* Container health checks
* Environment-variable configuration
* .NET User Secrets support for local development

---

## Observability and Production Tooling

CVCrafta is being developed toward a production-oriented cloud architecture, not only as a local full-stack application.

The planned observability and infrastructure stack includes:

| Area                         | Technology           | Purpose                                                                                    |
| ---------------------------- | -------------------- | ------------------------------------------------------------------------------------------ |
| Telemetry                    | OpenTelemetry        | Collect application traces, metrics, and telemetry using vendor-neutral instrumentation    |
| Metrics                      | Prometheus           | Collect and query application and infrastructure metrics                                   |
| Visualization                | Grafana              | Build dashboards for application health, performance, and operational metrics              |
| Azure Observability          | Application Insights | Monitor application performance, failures, dependencies, and distributed requests in Azure |
| Cloud Platform               | Microsoft Azure      | Host and operate the production application and supporting infrastructure                  |
| Infrastructure as Code       | Terraform            | Provision and version cloud infrastructure through code                                    |
| Azure Infrastructure as Code | Bicep                | Define Azure-native infrastructure declaratively                                           |
| Containers                   | Docker               | Package application services consistently across environments                              |
| Container Orchestration      | Kubernetes           | Support future container orchestration and scalable deployment scenarios                   |
| Automation                   | GitHub Actions       | Automate build, testing, validation, and deployment workflows                              |
| Delivery                     | CI/CD                | Provide repeatable automated delivery from source control to deployment environments       |

### Planned Observability Flow

```text
CVCrafta
│
├── React Client
│
└── ASP.NET Core API
        │
        ▼
   OpenTelemetry
        │
        ├────────► Prometheus
        │              │
        │              ▼
        │           Grafana
        │
        └────────► Azure Application Insights
```

The goal is to make application behavior observable across the full request lifecycle, including API performance, errors, dependencies, database activity, cache behavior, and production health.

These capabilities are part of the planned production hardening of CVCrafta and will be introduced incrementally without disrupting currently working application logic.


## Technology Stack

| Area                   | Technology                  |
| ---------------------- | --------------------------- |
| Backend                | ASP.NET Core 10 Web API     |
| Language               | C#                          |
| Frontend               | React 19                    |
| Frontend tooling       | Vite 7                      |
| Styling                | Tailwind CSS 4              |
| Database               | Microsoft SQL Server        |
| ORM                    | Entity Framework Core 10    |
| Authentication         | ASP.NET Core Identity + JWT |
| Caching                | Redis                       |
| Email                  | MailKit / SMTP              |
| API documentation      | Swagger / OpenAPI           |
| Containers             | Docker + Docker Compose     |
| Frontend data fetching | TanStack Query + Axios      |
| Frontend testing       | Vitest + Testing Library    |
| Backend testing        | .NET automated tests        |

---

## Project Structure

```text
CVCrafta/
│
├── .github/
│   └── workflows/
│
├── docs/
│   └── email-delivery.md
│
├── resumeapp.client/
│   ├── src/
│   ├── package.json
│   └── ...
│
├── ResumeApp.Server/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Model/
│   ├── Services/
│   ├── Dockerfile
│   └── ResumeApp.Server.csproj
│
├── ResumeApp.Server.Tests/
│
├── .env.example
├── .gitignore
├── docker-compose.yml
├── docker-compose.override.yml
├── ResumeApp.slnx
└── README.md
```

> **Note:** CVCrafta is the public-facing product name. Some internal project names and namespaces still use `ResumeApp` while the project is being prepared for open-source release.

---

## Architecture Overview

CVCrafta uses a separated frontend and backend architecture.

```text
┌─────────────────────┐
│    React Client     │
│ React + Vite        │
└─────────┬───────────┘
          │ HTTP / JSON
          ▼
┌─────────────────────┐
│ ASP.NET Core Web API│
│ Controllers / DTOs  │
│ Services / Identity │
└─────┬─────────┬─────┘
      │         │
      │         └──────────────► Redis
      │                          Distributed Cache
      │
      ▼
┌─────────────────────┐
│     SQL Server      │
│ Entity Framework   │
│ Core               │
└─────────────────────┘
```

Authentication is handled by ASP.NET Core Identity and JWT bearer tokens. Persistent application data is stored in SQL Server, while Redis provides distributed caching capabilities.

---

## API

The backend exposes REST API endpoints for authentication and resume management.

Primary endpoint groups include:

```text
/api/auth
/api/resume
```

Swagger/OpenAPI is available in development for exploring and testing the API.

When the application is running through the Docker Compose development profile, Swagger can be accessed through the configured server address.

---

## Email Confirmation

CVCrafta includes an email-confirmation workflow designed to handle confirmation across different browser contexts.

A user can confirm their email from another tab or window and have the application recognize the updated account state when they return.

Email delivery can use development or SMTP-based providers depending on configuration.

Additional documentation is available here:

```text
docs/email-delivery.md
```

---

## Getting Started

### Prerequisites

Install the following before running CVCrafta:

* .NET 10 SDK
* Visual Studio with ASP.NET and web development support
* Node.js
* Docker Desktop
* Git

The Docker-based development environment provides SQL Server and Redis automatically.

---

## Clone the Repository

```powershell
git clone <repository-url>
cd ResumeApp
```

The public repository URL will be added when CVCrafta is ready for release.

---

## Environment Configuration

CVCrafta does not store production or developer secrets in source control.

Copy the supplied environment template:

```powershell
Copy-Item .env.example .env
```

Then provide appropriate local values for the variables defined in `.env.example`.

Typical Docker configuration includes:

```text
SQL_SA_PASSWORD=<your-strong-local-password>
JWT_KEY=<your-long-random-jwt-signing-key>
```

Never commit the populated `.env` file.

For non-containerized ASP.NET Core development, sensitive configuration can also be stored using .NET User Secrets.

Example:

```powershell
cd ResumeApp.Server

dotnet user-secrets set "Jwt:Key" "<your-jwt-key>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-local-connection-string>"
```

---

## Run with Visual Studio and Docker Compose

CVCrafta is configured to support Visual Studio Docker Compose development.

Open:

```text
ResumeApp.slnx
```

in Visual Studio.

Select the **Docker Compose** launch profile and start the application.

Docker Compose starts the required services:

```text
resumeapp.server
sqlserver
redis
```

SQL Server uses a persistent Docker volume so application data can survive normal container restarts.

The SQL Server container also includes a health check, and the application waits for its required infrastructure before starting.

---

## Run the Frontend Separately

From the repository root:

```powershell
cd resumeapp.client
npm install
npm run dev
```

The Vite development server will start the React frontend.

---

## Run the Backend Separately

Ensure SQL Server and Redis are available and the required local configuration has been provided.

Then:

```powershell
cd ResumeApp.Server
dotnet restore
dotnet run
```

For the normal CVCrafta development workflow, Visual Studio is recommended.

---

## Testing

### Backend

Run the .NET test suite from the repository root:

```powershell
dotnet test
```

The backend test project is:

```text
ResumeApp.Server.Tests
```

The test suite covers application behavior including authentication, validation, authorization, ownership rules, resume operations, and related API behavior.

### Frontend

Run the React test suite with:

```powershell
npm --prefix .\resumeapp.client run test:run
```

The frontend uses Vitest and React Testing Library.

---

## Docker Services

The development Docker Compose configuration currently includes three primary services:

| Service            | Purpose                  |
| ------------------ | ------------------------ |
| `resumeapp.server` | ASP.NET Core Web API     |
| `sqlserver`        | SQL Server 2022 database |
| `redis`            | Redis distributed cache  |

The API container receives sensitive configuration through environment variables rather than hard-coded credentials.

---

## Security

Security-related practices used by the project include:

* JWT bearer authentication
* ASP.NET Core Identity
* Password hashing through Identity
* Authorization on protected API endpoints
* Resource ownership validation
* Email confirmation
* DTO validation
* Environment-based secret management
* .NET User Secrets for local development
* `.env` exclusion from Git
* Secret scanning during repository preparation
* No production credentials stored in tracked configuration

Security issues should not be disclosed publicly before the repository's security reporting process is finalized.

---

## Development Principles

CVCrafta is being developed with an emphasis on:

* Clear separation of concerns
* Maintainable backend services
* Secure authentication and authorization
* Automated testing
* Reproducible development environments
* Containerized infrastructure
* Explicit configuration management
* Incremental production hardening
* Contributor-friendly documentation

---

## Current Status

CVCrafta is under active development and is currently being prepared for public open-source release.

The core full-stack application is functional, including authentication, email confirmation, resume management, automated frontend and backend testing, SQL Server persistence, Redis integration, and Docker-based local infrastructure.

Additional work is focused on open-source documentation, CI/CD hardening, contributor onboarding, observability, deployment preparation, and repository presentation.

---

## Roadmap

Planned areas of continued development include:

* Open-source contributor documentation
* Continuous integration improvements
* GitHub Actions CI/CD pipeline hardening
* Security automation
* OpenTelemetry instrumentation
* Prometheus metrics collection
* Grafana dashboards
* Azure Application Insights integration
* Microsoft Azure deployment
* Infrastructure as Code with Terraform
* Azure infrastructure provisioning with Bicep
* Kubernetes deployment where appropriate
* Production logging and monitoring
* Performance monitoring and optimization
* Deployment automation
* Additional resume features
* Further frontend UX improvements
* Expanded automated testing

---

## Contributing

CVCrafta is being prepared to accept community contributions.

Contributor guidelines, issue templates, development setup instructions, pull-request expectations, and security guidance will be finalized before the repository is publicly opened for contributions.

Until then, please treat the repository as pre-release.

---

## Documentation

Additional project documentation is stored in the [`docs`](./docs) directory.

Current documentation includes:

* [Email Delivery](./docs/email-delivery.md)

---

## License

A license will be finalized before the public open-source release.

---

## About the Project

CVCrafta is both a practical full-stack application and an engineering project focused on demonstrating modern C#/.NET development practices beyond basic CRUD functionality.

The project is being developed around real production concerns such as authentication, authorization, email verification, distributed caching, containerized infrastructure, automated testing, secure configuration, observability, and maintainable application architecture.
