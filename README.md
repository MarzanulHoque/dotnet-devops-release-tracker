# 🚀 DevOps Deployment Tracker — Containerized Angular SPA & REST API Architecture

[![CI/CD Build & Test](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions/workflows/deploy.yml/badge.svg)](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions)
[![Architecture](https://img.shields.io/badge/Architecture-Docker%20Compose%203--Tier-blueviolet)](#-containerized-cloud--vm-architecture)
[![Docker](https://img.shields.io/badge/Docker-Multi--Stage%20Builds-2496ED?logo=docker&logoColor=white)](#-docker-orchestration)
[![Angular](https://img.shields.io/badge/Angular-20%20Standalone-DD0031?logo=angular)](#-architectural-principles)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4?logo=dotnet)](#-architectural-principles)
[![MySQL](https://img.shields.io/badge/Database-MySQL%208.0-4479A1?logo=mysql)](#-architectural-principles)
[![Swagger OpenAPI](https://img.shields.io/badge/API%20Docs-Swagger%20OpenAPI-success?logo=swagger)](#-key-features)
[![VMware Ready](https://img.shields.io/badge/VMware-Workstation%20%2F%20Linux%20VM-607078?logo=vmware&logoColor=white)](#-local--vmware-quickstart)

> 💡 **Cloud & Local Homelab DevOps:** The platform can be deployed on AWS EC2 or locally on **VMware Workstation / Linux VMs** with full multi-container isolation via Docker Compose.
> 
> ⚙️ **Production Stack:** Angular 20 Standalone SPA (:80 via Nginx) • ASP.NET Core 8.0 REST API Microservice (:5050) • Pomelo MySQL 8.0 Relational Persistence (:3306) • Docker Compose Orchestration

---

## 🏛️ Containerized Cloud & VM Architecture

The entire ecosystem is orchestrated with **Docker Compose**, running as isolated, networked microservices:

```
[ Browser / Public Client ]
             │
             ▼ (Port 80)
   ┌────────────────────────────────────────────────────────────────────────┐
   │                     Docker Compose Bridge Network                      │
   │                                                                        │
   │  ┌────────────────────────┐         ┌───────────────────────────────┐  │
   │  │   dotnet-frontend      │         │          dotnet-api           │  │
   │  │   (Angular 20 SPA)     │         │      (ASP.NET Core 8.0)       │  │
   │  │                        │         │                               │  │
   │  │  • Nginx Alpine (:80)  │────────►│  • Kestrel Microservice       │  │
   │  │  • Static Asset Host   │ /api/   │    (:5050)                    │  │
   │  │  • Internal Reverse    │ proxy   │  • Swagger OpenAPI (/swagger) │  │
   │  │    Proxy to API        │         │  • Automatic EF Migrations    │  │
   │  └────────────────────────┘         └───────────────┬───────────────┘  │
   │                                                     │                  │
   │                                                     │ (Pomelo EF Core) │
   │                                                     ▼                  │
   │                                     ┌───────────────────────────────┐  │
   │                                     │           dotnet-db           │  │
   │                                     │          (MySQL 8.0)          │  │
   │                                     │                               │  │
   │                                     │  • Persistent Volume (db_data)│  │
   │                                     │  • Automatic Healthcheck      │  │
   │                                     └───────────────────────────────┘  │
   └────────────────────────────────────────────────────────────────────────┘
```

### Architectural Principles:
1. **Frontend Presentation Tier (`frontend/`)**:
   - Built with **Angular 20 Standalone Components**, eliminating legacy NgModules.
   - Reactive state management powered by **Angular Signals** (`signal`, `computed`) for live telemetry computation.
   - Packaged with a multi-stage **`frontend/Dockerfile`** and served via an optimized **Nginx Alpine** runtime (~25MB).
   - Built-in **Nginx Reverse Proxy** forwards all `/api/` traffic to the backend container, eliminating cross-origin (CORS) concerns.
2. **Business / REST API Microservice Tier (`DotnetProject.Api`)**:
   - Containerized via multi-stage **`DotnetProject.Api/Dockerfile`** with .NET 8.0 SDK on Alpine (~100MB final runtime image).
   - Autonomous RESTful Web API listening on port `5050`.
   - Automatic database schema creation and seed data execution on startup via `context.Database.Migrate()`.
   - Exposes `POST /api/deployments/webhook` to ingest automated deployment telemetry directly from CI/CD pipelines.
3. **Domain & Data Access Tier (`DotnetProject.Core` & MySQL 8.0)**:
   - Centralized relational domain: `Projects`, `Deployments`, and `DeploymentLogs`.
   - MySQL 8.0 container with persistent Docker volume (`db_data`) ensuring zero data loss across container recycles.
   - Healthcheck integration ensures the API container waits until MySQL is fully ready before launching.

---

## ✨ Key Features

- **Multi-Stage Docker Builds:** Ultra-lean production images for both Angular (.NET build: ~100MB; Angular/Nginx: ~25MB).
- **Single-Command Orchestration:** `docker compose up -d --build` spins up database, backend, and frontend with a single command.
- **Dynamic Secrets Management:** Credentials loaded safely via `.env` file (never committed to git) with `.env.example` template provided.
- **Automated Startup EF Core Migrations:** Zero manual SQL scripts required; schema upgrades and initial seed data execute idempotently on boot.
- **Real-Time Telemetry Dashboard:** Live deployment statistics, environment breakdowns, and animated architecture topologies.
- **Fail-Safe CI Quality Gates:** Automated xUnit test suite verifies domain models, controllers, and API contracts.

---

## 📂 Solution Project Structure

```
.
├── docker-compose.yml                   # Multi-container orchestration (db, api, frontend)
├── .env.example                         # Environment secrets template for deployment
├── .dockerignore                        # Optimized build context filtering
├── DotnetProject.Core/                  # Shared Domain & Data Library
│   ├── Data/
│   │   ├── ApplicationDbContext.cs      # EF Core MySQL mapping & seed data
│   │   └── ApplicationDbContextFactory.cs # Design-time factory for EF Core migrations
│   ├── DTOs/                            # Data Transfer Objects (CRUD, Webhooks, Responses)
│   ├── Entities/                        # Project, DeploymentRecord, DeploymentLog
│   └── Migrations/                      # EF Core Code-First migration snapshots
├── DotnetProject.Api/                   # Tier 2: RESTful Web API (:5050)
│   ├── Controllers/
│   │   ├── DeploymentsController.cs     # CRUD & CI/CD webhook endpoint
│   │   ├── HealthController.cs          # API diagnostics & database probe
│   │   └── ProjectsController.cs        # Registered applications catalog
│   ├── Dockerfile                       # Multi-stage Alpine container build
│   └── Program.cs                       # Swagger, CORS, and automatic DB migration
├── frontend/                            # Tier 1: Modern Angular 20 Standalone SPA (:80)
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/                    # Models & Signals Services (Deployment, Project, Health)
│   │   │   ├── shared/                  # Navbar with Heartbeat, Footer, StatusBadge
│   │   │   └── features/                # Dashboard, Deployments CRUD & Diagnostics views
│   │   └── styles.css                   # Bootstrap 5.3 + Glassmorphic Telemetry Theme
│   ├── Dockerfile                       # Multi-stage Node.js + Nginx Alpine build
│   └── nginx.conf                       # SPA routing fallback and API proxy configuration
└── tests/DotnetProject.Tests/           # xUnit Test Suite (12/12 Passed)
    ├── DeploymentsApiControllerTests.cs # Integration tests for REST API endpoints
    ├── DeploymentRecordModelTests.cs    # Domain model validation tests
    └── HealthCheckTests.cs              # Health diagnostics verification
```

---

## 🚀 Local & VMware Quickstart

### Method 1: Docker Compose (Recommended for Local & VMware VMs)

#### Prerequisites:
- [Docker Engine & Docker Compose](https://docs.docker.com/engine/install/) (or Docker Desktop)
- Git

#### 1. Clone & Configure Secrets:
```bash
git clone https://github.com/marzanulhoque/dotnet-devops-release-tracker.git
cd dotnet-devops-release-tracker

# Create your private environment configuration from template
cp .env.example .env
```

#### 2. Launch Stack:
```bash
docker compose up -d --build
```

#### 3. Verify Containers:
```bash
docker compose ps
```

* **Angular UI Dashboard:** `http://localhost/` (or `http://<VM-IP-ADDRESS>/`)
* **Swagger OpenAPI Docs:** `http://localhost:5050/swagger`
* **API Health Diagnostics:** `http://localhost:5050/api/health`

---

### Method 2: Local Development (Without Docker)

#### Prerequisites:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+ / npm](https://nodejs.org/)

#### 1. Run Automated Tests:
```bash
dotnet test --nologo
```

#### 2. Launch Services:
```bash
# Terminal 1: Launch REST API (:5050)
dotnet run --project DotnetProject.Api

# Terminal 2: Launch Angular Standalone SPA (:4200)
cd frontend
npm install
npm start
```

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
- [x] **Phase 4: Docker Containerization & VMware Deployment (`feat/phase3-docker-containerization`)**
  - Multi-stage Dockerfiles for both frontend (Angular + Nginx) and backend (.NET 8).
  - Docker Compose orchestration with healthchecks, persistent volumes, and `.env` secrets management.
  - Deployment-ready on local development environments, VMware Workstation Linux VMs, and cloud hypervisors.
- [ ] **Phase 5: Automated CI/CD with Self-Hosted Runner on VMware (Upcoming)**
  - Automated continuous delivery pipeline triggering on pushes to `main` with live status reporting.

---

## 👤 Author

**S. M. MARZANUL HOQUE**
- Email: [marzanulru17@gmail.com](mailto:marzanulru17@gmail.com)
- GitHub: [@marzanulhoque](https://github.com/marzanulhoque)

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
