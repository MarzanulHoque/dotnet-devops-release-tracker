# 🚀 DevOps Deployment Tracker — True 3-Tier Architecture on AWS EC2

[![CI/CD Build & Test](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions/workflows/deploy.yml/badge.svg)](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions)
[![Architecture](https://img.shields.io/badge/Architecture-True%203--Tier%20Decoupled-blueviolet)](#-true-3-tier-architecture)
[![Swagger OpenAPI](https://img.shields.io/badge/API%20Docs-Swagger%20OpenAPI-success?logo=swagger)](#-key-features)
![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4?logo=dotnet)
![AWS EC2](https://img.shields.io/badge/AWS-EC2%20Linux-FF9900?logo=amazonec2)
![MySQL](https://img.shields.io/badge/Database-MySQL%208.0-4479A1?logo=mysql)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?logo=githubactions)
![systemd](https://img.shields.io/badge/Daemon-systemd%20sandboxed-black?logo=linux)

> 💡 **Cloud Cost Optimization Notice:** The AWS EC2 demonstration instance was stopped after milestone validation to eliminate idle cloud costs. The full 3-tier decoupled ecosystem can be deployed on-demand via our automated CI/CD pipeline or run locally using the instructions below.
> 
> ⚙️ **Production Stack:** AWS EC2 Linux (Ubuntu 24.04 LTS) • Nginx Reverse Proxy (:80) • Dual systemd Daemons with Container-Grade Sandboxing (`DynamicUser=yes`, `ProtectSystem=strict`) • MySQL 8.0 with EF Core Code-First Migrations

---

## 🏛️ True 3-Tier Architecture

In **Phase 2**, we evolved our monolithic MVC application into a **True 3-Tier Decoupled Architecture**:

```
[ Browser / Public Client ]
             │
             ▼ (Public Port 80)
      [ Nginx Reverse Proxy ]
             ├── location / ─────────────────► [ MVC Presentation Tier (:5000) ]
             │                                              │
             │                                              │ (Internal HTTP via IDeploymentApiClient)
             ▼                                              ▼
   [ External CI/CD Webhooks ] ──────► [ REST API Business Tier (:5050) ] ◄─── [ Swagger OpenAPI ]
                                                            │
                                                            │ (Entity Framework Core Pomelo)
                                                            ▼
                                                [ MySQL 8.0 Database Tier ]
```

### Architectural Principles:
1. **Pure Presentation Tier (`DotnetProject.Web` / MVC)**:
   - Contains **zero database dependencies, zero MySQL drivers, and zero database connection strings**.
   - Connects to the business tier exclusively over HTTP via a strongly-typed `IDeploymentApiClient`.
   - Features a modern **glassmorphic dark-mode UI**, real-time architecture telemetry, and an **interactive in-browser REST API explorer**.
2. **Business / Services Tier (`DotnetProject.Api`)**:
   - Dedicated RESTful Web API listening internally on port `5050`.
   - Implements full release CRUD operations, query filtering, and real-time system diagnostics (`/api/health`).
   - Hosts interactive OpenAPI documentation at `/swagger`.
   - Exposes `POST /api/deployments/webhook` to ingest automated deployment telemetry directly from CI/CD pipelines (GitHub Actions, GitLab CI, Jenkins).
3. **Domain & Data Access Tier (`DotnetProject.Core`)**:
   - Centralized relational domain: `Projects`, `Deployments`, and `DeploymentLogs` with foreign key relationships and cascade rules.
   - Code-First EF Core migrations with `ApplicationDbContextFactory` for design-time scaffolding.
   - Automatic startup migrations via `context.Database.Migrate()`.

---

## ✨ Key Features

- **True 3-Tier Decoupling:** Complete separation of presentation (MVC), business logic/integration (Web API), and persistence (MySQL).
- **Container-Grade Linux Isolation on Bare EC2:** Both services run under native `systemd` with `DynamicUser=yes`, `ProtectSystem=strict`, `ProtectHome=yes`, `NoNewPrivileges=true`, and `PrivateTmp=true` — delivering container-grade sandboxing without Docker daemon RAM overhead.
- **Automated CI/CD Webhook Ingestion:** External pipeline bots post release metadata directly to `/api/deployments/webhook`.
- **Interactive Swagger OpenAPI Documentation:** Live API exploration at `/swagger`.
- **Automated Startup EF Core Migrations:** Zero manual SQL scripts required; schema upgrades execute idempotently via `context.Database.Migrate()`.
- **Zero Secrets in Source Control:** Database credentials reside strictly in server-only `/etc/dotnetapp.env` (`chmod 600`), dynamically loaded by the API daemon.
- **Fail-Safe CI Quality Gates:** 16 automated xUnit tests run on every push, testing models, controllers, and API contracts.

---

## 🔒 Security & Linux Kernel Sandboxing

```
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                                 AWS Linux EC2 Server Host                                   │
│                                                                                             │
│  1. Server Secrets File: /etc/dotnetapp.env (chmod 600, www-data owned)                    │
│     └── ConnectionStrings__DefaultConnection="Server=...;User=...;Password=<RANDOM_GEN>;"  │
│                                                                                             │
│  2. Linux systemd Kernel Sandboxing (dotnetapi.service & dotnetapp.service)                │
│     ├── DynamicUser=yes       ──► Ephemeral UID dynamically allocated per run               │
│     ├── ProtectSystem=strict  ──► Entire root filesystem is mounted strictly read-only      │
│     ├── ProtectHome=yes       ──► /home and /root are completely inaccessible               │
│     ├── NoNewPrivileges=true  ──► Disables privilege escalation exploits                    │
│     └── PrivateTmp=true       ──► Isolated temporary file namespace                         │
│                                                                                             │
│  3. Host Ingress Boundary (Nginx)                                                           │
│     └── Public Port 80 only. Internal Kestrel ports 5000 & 5050 bound strictly to 127.0.0.1 │
└─────────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 📂 Solution Project Structure

```
.
├── .github/workflows/
│   └── deploy.yml                       # Dual-daemon CI/CD pipeline for main & feat/** branches
├── DotnetProject.Core/                  # Shared Domain & Data Library
│   ├── Data/
│   │   ├── ApplicationDbContext.cs      # EF Core MySQL mapping & seed data
│   │   └── ApplicationDbContextFactory.cs # Design-time factory for EF Core migrations
│   ├── DTOs/                            # Transfer models (CRUD, Webhooks, Responses)
│   ├── Entities/                        # Project, DeploymentRecord, DeploymentLog
│   └── Migrations/                      # Compiled EF Core Code-First migration snapshots
├── DotnetProject.Api/                   # Tier 2: RESTful Web API (:5050)
│   ├── Controllers/
│   │   ├── DeploymentsController.cs     # CRUD & CI/CD webhook endpoint
│   │   ├── HealthController.cs          # API diagnostics & database probe
│   │   └── ProjectsController.cs        # Registered applications catalog
│   └── Program.cs                       # Swagger, CORS, and automatic DB migration
├── MVC/                                 # Tier 1: Pure Presentation Portal (:5000)
│   ├── Controllers/
│   │   ├── DeploymentsController.cs     # Delegates all actions to IDeploymentApiClient
│   │   ├── HealthController.cs          # Probes downstream API tier health
│   │   └── HomeController.cs            # Overview & system topology
│   ├── Services/
│   │   ├── IDeploymentApiClient.cs      # Contract for REST API consumption
│   │   └── DeploymentApiClient.cs       # Typed HttpClient implementation
│   ├── Views/                           # Modern Glassmorphic Razor views
│   │   ├── Home/Index.cshtml            # Dashboard with interactive in-browser API explorer
│   │   └── Deployments/Index.cshtml     # Releases table with CI webhook cURL generator
│   └── Program.cs                       # Configures AddHttpClient<IDeploymentApiClient>()
├── tests/DotnetProject.Tests/           # xUnit Test Suite (16/16 Passed)
│   ├── DeploymentsApiControllerTests.cs # Integration tests for REST API endpoints
│   ├── DeploymentsControllerTests.cs    # Unit tests for MVC controller with FakeApiClient
│   └── HealthCheckTests.cs              # Health diagnostics verification
└── deploy/                              # Infrastructure as Code (IaC)
    ├── api.service                      # systemd unit for DotnetProject.Api (:5050)
    ├── app.service                      # systemd unit for DotnetProject.Web (:5000)
    ├── nginx.conf                       # Reverse proxy path-routing configuration
    └── setup-ec2.sh                     # Idempotent EC2 bootstrap script
```

---

## 🌐 Nginx Reverse Proxy Configuration

Nginx on port 80 routes incoming requests based on path:

```nginx
# 1. MVC Frontend Dashboard (Port 5000)
location / {
    proxy_pass http://127.0.0.1:5000;
}

# 2. REST Web API Endpoints (Port 5050)
location /api {
    proxy_pass http://127.0.0.1:5050;
}

# 3. Interactive Swagger OpenAPI Docs (Port 5050)
location /swagger {
    proxy_pass http://127.0.0.1:5050;
}
```

---

## 🚀 Local Development Quickstart

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### 1. Build & Run Automated Tests
```bash
# Clone repository
git clone https://github.com/marzanulhoque/dotnet-devops-release-tracker.git
cd dotnet-devops-release-tracker

# Build and execute all 16 test gates
dotnet build DotnetProject.sln
dotnet test DotnetProject.sln --configuration Release
```

### 2. Launch Services
```bash
# Terminal 1: Launch REST API (:5050)
cd DotnetProject.Api
dotnet run

# Terminal 2: Launch MVC Web Dashboard (:5000)
cd MVC
dotnet run
```

- **Web Dashboard:** `http://localhost:5000/`
- **Swagger Documentation:** `http://localhost:5050/swagger`
- **API Diagnostics:** `http://localhost:5050/api/health`

---

## 📈 Evolutionary Roadmap

- [x] **Phase 1: ASP.NET Core 8.0 MVC Monolith (Completed)**
  - Single process MVC application, Razor dashboard, xUnit test gates, and GitHub Actions CI/CD to AWS EC2.
- [x] **Phase 2: True 3-Tier Architecture & Relational Domain (Current Branch)**
  - Presentation Tier decoupled from DB via `IDeploymentApiClient`.
  - Dedicated REST Web API with Swagger, CI/CD webhook ingestion, and Pomelo MySQL EF Core migrations.
  - Dual systemd daemons with container-grade sandboxing (`DynamicUser=yes`, `ProtectSystem=strict`).
- [ ] **Phase 3: Dedicated Data Access Layer (DAL) & Clean Architecture (Upcoming)**
  - Extraction into `DotnetProject.Data` with Repository & Unit of Work patterns.

---

## 👤 Author

**S. M. MARZANUL HOQUE**
- Email: [marzanulru17@gmail.com](mailto:marzanulru17@gmail.com)
- GitHub: [@marzanulhoque](https://github.com/marzanulhoque)

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
