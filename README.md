# 🚀 DevOps Deployment Tracker — Decoupled Angular SPA & REST API Architecture

[![CI/CD Build & Test](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions/workflows/deploy.yml/badge.svg)](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions)
[![Architecture](https://img.shields.io/badge/Architecture-Angular%20SPA%20%2B%20REST%20API-blueviolet)](#-phase-3-decoupled-spa-architecture)
[![Swagger OpenAPI](https://img.shields.io/badge/API%20Docs-Swagger%20OpenAPI-success?logo=swagger)](#-key-features)
![Angular](https://img.shields.io/badge/Angular-20%20Standalone-DD0031?logo=angular)
![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4?logo=dotnet)
![AWS EC2](https://img.shields.io/badge/AWS-EC2%20Linux-FF9900?logo=amazonec2)
![MySQL](https://img.shields.io/badge/Database-MySQL%208.0-4479A1?logo=mysql)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?logo=githubactions)

> 💡 **Cloud Cost Optimization Notice:** The AWS EC2 demonstration instance was stopped after milestone validation to eliminate idle cloud costs. The full decoupled ecosystem can be deployed on-demand via our automated CI/CD pipeline or run locally using the instructions below.
> 
> ⚙️ **Phase 3 Stack:** Angular 20 Standalone SPA (:4200) • ASP.NET Core 8.0 REST API Microservice (:5050) • Pomelo MySQL 8.0 Relational Persistence • Nginx / Docker Container Ready

---

## 🏛️ Phase 3 Decoupled SPA Architecture

In **Phase 3**, the legacy monolithic MVC layer has been converted into an enterprise-grade **Angular 20 Single-Page Application (SPA)**:

```
[ Browser / Single-Page Application ]
                 │
                 ▼ (Port 4200 in Dev / Port 80 via Reverse Proxy)
     [ Angular 20 Standalone SPA ]
                 │
                 │ (HTTP / JSON over CORS with Angular Signals)
                 ▼
   [ REST API Microservice Tier (:5050) ] ◄─── [ External CI/CD Webhooks ]
                 │                                        ▲
                 │ (Entity Framework Core Pomelo)         │
                 ▼                              [ Swagger OpenAPI Docs ]
     [ MySQL 8.0 Database Tier ]
```

### Architectural Principles:
1. **Modern Frontend SPA (`frontend/`)**:
   - Built with **Angular 20 Standalone Components**, eliminating legacy NgModules.
   - Reactive state management powered by **Angular Signals** (`signal`, `computed`) for live telemetry computation.
   - Modern dark glassmorphic design system using **Bootstrap 5.3 + Bootstrap Icons**.
   - Dynamic real-time API health heartbeat indicator and telemetry data table with client-side filtering, sorting, and pagination.
2. **Autonomous Business / REST API Tier (`DotnetProject.Api`)**:
   - Autonomous RESTful Web API listening on port `5050` with open CORS for the SPA.
   - Implements full release CRUD operations, query filtering, and real-time system diagnostics (`/api/health`).
   - Hosts interactive OpenAPI documentation at `/swagger`.
   - Ingests automated deployment events directly from CI/CD pipelines via `POST /api/deployments/webhook`.
3. **Domain & Data Access Tier (`DotnetProject.Core`)**:
   - Centralized relational domain: `Projects`, `Deployments`, and `DeploymentLogs`.
   - Code-First EF Core migrations with automatic startup synchronization via `context.Database.Migrate()`.

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
├── frontend/                            # Tier 1: Modern Angular 20 Standalone SPA (:4200)
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                    # Models (Deployment, Project, Health) & Signals Services
│   │   │   ├── shared/                  # Navbar with Heartbeat, Footer, StatusBadge
│   │   │   └── features/                # Dashboard, Deployments CRUD & Diagnostics views
│   │   ├── environments/                # Environment configurations (API Base URL)
│   │   └── styles.css                   # Bootstrap 5.3 + Glassmorphic Telemetry Theme
│   └── angular.json                     # Standalone application build configuration
├── tests/DotnetProject.Tests/           # xUnit Test Suite (12/12 Passed)
│   ├── DeploymentsApiControllerTests.cs # Integration tests for REST API endpoints
│   ├── DeploymentRecordModelTests.cs    # Domain model validation tests
│   └── HealthCheckTests.cs              # Health diagnostics verification
└── deploy/                              # Infrastructure as Code (IaC)
    ├── api.service                      # systemd unit for DotnetProject.Api (:5050)
    ├── nginx.conf                       # Reverse proxy path-routing configuration
    └── setup-ec2.sh                     # Idempotent EC2 bootstrap script
```

---

## 🚀 Local Development Quickstart

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+ / npm](https://nodejs.org/)
- Git

### 1. Build & Run Automated Tests
```bash
# Clone repository
git clone https://github.com/marzanulhoque/dotnet-devops-release-tracker.git
cd dotnet-devops-release-tracker

# Run .NET backend and integration tests
dotnet test --nologo

# Run Angular frontend build
cd frontend
npm install
npm run build
cd ..
```

### 2. Launch Services Locally
```bash
# Terminal 1: Launch ASP.NET Core REST API (:5050)
dotnet run --project DotnetProject.Api

# Terminal 2: Launch Angular Standalone SPA (:4200)
cd frontend
npm start
```

- **Angular UI Dashboard:** `http://localhost:4200/`
- **Swagger Documentation:** `http://localhost:5050/swagger`
- **API Health Diagnostics:** `http://localhost:5050/api/health`

---

## 📈 Evolutionary Roadmap

- [x] **Phase 1: ASP.NET Core 8.0 MVC Monolith (Completed)**
  - Single process MVC application, Razor dashboard, xUnit test gates, and GitHub Actions CI/CD to AWS EC2.
- [x] **Phase 2: True 3-Tier Architecture & Relational Domain (Completed)**
  - Dedicated REST Web API (`DotnetProject.Api`) with Swagger, CI/CD webhook ingestion, and Pomelo MySQL EF Core migrations.
  - Dual systemd daemons with container-grade sandboxing (`DynamicUser=yes`, `ProtectSystem=strict`).
- [x] **Phase 3: Decoupled Angular Standalone SPA (`feat/phase3-angular-frontend`)**
  - Converted UI into Angular 20 Standalone SPA (`frontend/`) using Angular Signals and Bootstrap 5.3 telemetry theme.
  - Full CRUD operations, live API heartbeat probe, and deprecation of legacy MVC project.
- [ ] **Phase 4: Docker Containerization (`feat/phase3-docker-containerization` - Upcoming)**
  - Containerize the decoupled frontend and REST API using multi-stage Dockerfiles and compose orchestration.

---

## 👤 Author

**S. M. MARZANUL HOQUE**
- Email: [marzanulru17@gmail.com](mailto:marzanulru17@gmail.com)
- GitHub: [@marzanulhoque](https://github.com/marzanulhoque)

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
