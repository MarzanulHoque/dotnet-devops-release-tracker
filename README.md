# 🚀 DevOps Deployment Tracker — ASP.NET Core 8.0 on AWS EC2

[![CI/CD Build & Test](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions/workflows/deploy.yml/badge.svg)](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions)
[![Architecture](https://img.shields.io/badge/Architecture-Decoupled%20Multi--Service-blueviolet)](#-architecture--continuous-delivery-pipeline)
[![Swagger OpenAPI](https://img.shields.io/badge/API%20Docs-Swagger%20OpenAPI-success?logo=swagger)](#-key-features)
![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4?logo=dotnet)
![AWS EC2](https://img.shields.io/badge/AWS-EC2%20Linux-FF9900?logo=amazonec2)
![MySQL](https://img.shields.io/badge/Database-MySQL%208.0-4479A1?logo=mysql)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?logo=githubactions)
![systemd](https://img.shields.io/badge/Daemon-systemd-black?logo=linux)

> 💡 **Cloud Cost Optimization:** The AWS EC2 demonstration host was stopped after milestone testing to eliminate idle cloud costs. The complete decoupled ecosystem can be spun up on-demand via our automated CI/CD pipeline or reproduced locally using the included runbook.
> 
> ⚙️ **Cloud Architecture:** AWS EC2 Linux (Ubuntu 24.04 LTS) • Nginx Reverse Proxy (:80) • Dual systemd Daemons (`dotnetapp.service` :5000 & `dotnetapi.service` :5050) • MySQL 8.0 Database

A production-grade, decoupled **ASP.NET Core 8.0** ecosystem automated with **GitHub Actions CI/CD** targeting an **AWS Linux EC2** instance running native Kestrel managed by **`systemd`** daemons and reverse-proxied with **Nginx** (no container overhead). 

This project demonstrates **Phase 2** of our architectural evolution: transitioning from a monolithic application into a decoupled multi-service architecture with a dedicated RESTful API tier and shared domain library.

---

## 🏗️ Architecture & Continuous Delivery Pipeline

```
                                [ Public Client / Web Traffic ]
                                               │
                                               ▼
                              [ AWS EC2 Linux: Nginx (:80) ]
                               ├── location /          ──► [ Kestrel MVC Dashboard (:5000) ]
                               ├── location /api       ──► [ Kestrel REST Web API (:5050) ]
                               └── location /swagger   ──► [ Kestrel Swagger UI (:5050) ]
                                                                       │
                                  ┌────────────────────────────────────┴────────────────────────────────────┐
                                  ▼                                                                         ▼
                     [ DotnetProject.Core ]                                                      [ MySQL 8.0 Database ]
                     ├── Entities: Project, DeploymentRecord, DeploymentLog                      ├── Projects (App registry)
                     ├── DTOs (CRUD, Webhooks, Logs)                                             ├── Deployments (Releases)
                     └── ApplicationDbContext (Shared EF Core mapping)                           └── DeploymentLogs (Audit logs)
                                                                                                 (/etc/dotnetapp.env)
```

---

## ✨ Key Features

- **Live Multi-Service Cloud Deployment:** Active on AWS EC2 at [http://13.220.86.134/](http://13.220.86.134/) with Swagger UI at [/swagger](http://13.220.86.134/swagger) and health metrics at [/health](http://13.220.86.134/health).
- **Decoupled REST API Tier:** Dedicated `DotnetProject.Api` service exposing CRUD endpoints, project telemetry, and filtering.
- **Automated CI/CD Webhook Ingestion:** Endpoint (`/api/deployments/webhook`) allowing external GitHub Actions/GitLab CI pipelines to log deployment outcomes automatically.
- **Relational DevOps Domain:** 3 normalized database tables (`Projects`, `Deployments`, `DeploymentLogs`) with foreign key relationships and cascade rules.
- **Automated Fail-Safe CI Gates:** 16 automated xUnit tests run on every push. If any validation, controller, or API test fails, deployment halts immediately.
- **Dual Linux `systemd` Supervision:** Separate daemons (`dotnetapp.service` on :5000, `dotnetapi.service` on :5050) with independent crash recovery (`RestartSec=10`).
- **Nginx Reverse Proxy:** Terminating HTTP on port 80 and path-routing `/` to the MVC UI, and `/api` + `/swagger` to the REST API.
- **Secure Secret Separation:** Zero credentials in Git. Both daemons share the same server-side `/etc/dotnetapp.env` (`chmod 600`).

---

## 🔒 Security Architecture & Zero-Secret Management

A fundamental requirement of enterprise DevOps is that **no credentials, passwords, or secrets are ever committed to source control or exposed in CI logs**. This repository enforces a multi-layered security model:

```
                  ┌───────────────────────────────────────────────────────────┐
                  │                      Source Control (Git)                 │
                  │   appsettings.json: "DefaultConnection": ""               │
                  │   deploy.yml: Uses ${{ secrets.EC2_SSH_KEY }}             │
                  │   Docs/: Strictly excluded via .gitignore                 │
                  └─────────────────────────────┬─────────────────────────────┘
                                                │
                                                ▼ (dotnet publish)
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│                                 AWS Linux EC2 Server                                        │
│                                                                                             │
│  1. /etc/dotnetapp.env (chmod 600, www-data owned)                                          │
│     └── ConnectionStrings__DefaultConnection="Server=...;User=...;Password=<RANDOM_GEN>;"  │
│                                                                                             │
│  2. Linux systemd (app.service)                                                             │
│     ├── EnvironmentFile=-/etc/dotnetapp.env                                                 │
│     ├── User=www-data (Least-privilege unprivileged user)                                   │
│     ├── ProtectSystem=full (Sandboxes /usr, /boot, /etc from runtime tampering)             │
│     └── NoNewPrivileges=true (Blocks privilege escalation exploits)                         │
│                                                                                             │
│  3. Runtime Host Isolation                                                                  │
│     └── setup-ec2.sh generates dynamic 24-char cryptographic passwords on the host via:     │
│         DB_PASS=$(openssl rand -base64 24 | tr -dc 'a-zA-Z0-9' | head -c 24)                │
└─────────────────────────────────────────────────────────────────────────────────────────────┘
```

### 1. Zero Credentials in Git
- In [`appsettings.json`](file:///d:/DevOps%20Projects/DotnetProject/MVC/appsettings.json), the connection string is intentionally empty:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
  ```
- ASP.NET Core's hierarchical configuration system reads `ConnectionStrings__DefaultConnection` directly from the OS environment on EC2, overriding the empty configuration without touching code files.

### 2. Server-Side Secret Storage (`/etc/dotnetapp.env`)
- Production secrets reside strictly on the EC2 host inside `/etc/dotnetapp.env`.
- Access is locked down to POSIX permissions `600` (`-rw-------`), preventing unauthorized non-root users or neighboring processes from reading credentials.

### 3. Dynamic Password Generation
- The server bootstrap script ([`setup-ec2.sh`](file:///d:/DevOps%20Projects/DotnetProject/deploy/setup-ec2.sh)) never uses hardcoded default passwords. It dynamically generates randomized credentials at runtime on the server using `openssl rand -base64 24`.

### 4. Least-Privilege Execution & OS Sandboxing
- The Kestrel web application runs under the unprivileged `www-data` service account, never as `root`.
- The systemd unit file incorporates Linux kernel namespaces (`ProtectSystem=full`, `NoNewPrivileges=true`) to isolate the process from the rest of the operating system.

### 5. Resilient Offline Testing
- In `Program.cs`, if no MySQL connection string is injected (such as in local developer workstations or GitHub Actions CI test runners), the application gracefully falls back to an EF Core In-Memory provider, allowing 100% automated test execution without exposing live database endpoints.

## 📂 Project Structure

```
.
├── .github/
│   └── workflows/
│       └── deploy.yml              # Multi-stage GitHub Actions CI/CD workflow
├── MVC/                            # ASP.NET Core 8.0 MVC Project
│   ├── Controllers/
│   │   ├── DeploymentsController.cs # CRUD actions and dashboard filtering
│   │   ├── HealthController.cs     # /health diagnostics endpoint
│   │   └── HomeController.cs       # Overview dashboard & system info
│   ├── Data/
│   │   └── ApplicationDbContext.cs # EF Core DbContext with MySQL mapping & seeds
│   ├── Models/
│   │   └── DeploymentRecord.cs     # Domain entity & validation annotations
│   ├── Views/                      # Bootstrap 5 dark-mode Razor views
│   ├── Program.cs                  # Resilient DB injection, Nginx headers, middleware
│   └── appsettings.json            # Configuration (zero secrets committed)
├── tests/
│   └── DotnetProject.Tests/        # Automated xUnit Test Suite
│       ├── DeploymentRecordModelTests.cs
│       ├── DeploymentsControllerTests.cs
│       └── HealthCheckTests.cs
├── deploy/
│   ├── app.service                 # Linux systemd service unit definition
│   ├── nginx.conf                  # Nginx reverse proxy configuration
│   └── setup-ec2.sh                # One-command server bootstrap script
└── DotnetProject.sln               # Solution file
```

---

## 🚀 Local Development Quickstart

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### 1. Clone & Build
```bash
git clone https://github.com/marzanulhoque/dotnet-devops-release-tracker.git
cd dotnet-devops-release-tracker
dotnet restore DotnetProject.sln
dotnet build DotnetProject.sln
```

### 2. Run Automated Tests
```bash
dotnet test DotnetProject.sln
```
> **Note:** Tests utilize in-memory providers and run fully isolated in under 2 seconds without requiring an external database.

### 3. Run Application Locally
```bash
cd MVC
dotnet run
```
Open your browser at `http://localhost:5030` or `https://localhost:7028`.
- Dashboard: `http://localhost:5030/`
- Releases: `http://localhost:5030/Deployments`
- Health Diagnostics: `http://localhost:5030/health`

---

## ☁️ AWS EC2 Deployment Guide

### Step 1: Launch EC2 Instance
1. In the **AWS Console**, launch an EC2 instance:
   - **AMI:** Ubuntu Server 24.04 LTS (or 22.04 LTS)
   - **Instance Type:** `t2.micro` or `t3.micro` (Free Tier eligible)
   - **Security Group:** Inbound rules for **SSH (22)**, **HTTP (80)**, and **HTTPS (443)**.
   - **Key Pair:** Save your `.pem` key file.

### Step 2: Bootstrap Server via `setup-ec2.sh`
1. Connect to your EC2 instance via SSH:
   ```bash
   ssh -i /path/to/key.pem ubuntu@<YOUR_EC2_PUBLIC_IP>
   ```
2. Copy or clone the `deploy/` directory to the server:
   ```bash
   cd deploy
   chmod +x setup-ec2.sh
   ./setup-ec2.sh
   ```
   *The script automatically installs the .NET 8 runtime, Nginx, MySQL, creates the `/var/www/dotnetapp` directory with `www-data` permissions, generates `/etc/dotnetapp.env` for credentials, and enables `dotnetapp.service`.*

### Step 3: Configure GitHub Repository Secrets
In your GitHub repository, go to **Settings** $\rightarrow$ **Secrets and variables** $\rightarrow$ **Actions** and add:

| Secret Name | Description | Example |
| :--- | :--- | :--- |
| `EC2_HOST` | EC2 Public IPv4 or DNS | `54.210.88.12` |
| `EC2_USERNAME` | SSH username | `ubuntu` |
| `EC2_SSH_KEY` | Entire content of your `.pem` private key | `-----BEGIN RSA PRIVATE KEY----- ...` |

### Step 4: Push to Main & Deploy
```bash
git add .
git commit -m "feat: trigger deployment"
git push origin main
```
GitHub Actions will build, run test gates, transfer binaries via SCP to `/var/www/dotnetapp`, restart `dotnetapp.service`, and verify `/health`.

Once deployed, visit your live instance at `http://<YOUR_EC2_PUBLIC_IP>/` (or explore the live demo running at [http://13.220.86.134/](http://13.220.86.134/)).

---

## 🛠️ Server Troubleshooting & Operations

```bash
# Check application daemon status
sudo systemctl status dotnetapp.service

# View real-time application logs
sudo journalctl -u dotnetapp.service -f

# Restart application service manually
sudo systemctl restart dotnetapp.service

# Verify Nginx reverse proxy configuration
sudo nginx -t
sudo systemctl status nginx

# Test local health endpoint on server
curl http://localhost/health
```

---

## 📈 Evolutionary Roadmap

- [x] **Phase 1: ASP.NET Core 8.0 MVC Monolith (Completed)**
  - Direct MySQL EF Core mapping, Razor dashboard, xUnit test gates, and GitHub Actions CI/CD to AWS EC2.
- [x] **Phase 2: Decoupled REST Web API Tier & Relational Domain (Completed)**
  - Extracted business & data logic into shared `DotnetProject.Core`, created standalone `DotnetProject.Api` service with Swagger/OpenAPI, relational domain (`Projects`, `Deployments`, `DeploymentLogs`), and automated CI/CD webhooks.
- [ ] **Phase 3: Formal 3-Tier Clean Architecture (Upcoming)**
  - Full physical separation: Presentation Tier, Business Logic / API Tier, and Data Access Layer (`DotnetProject.Data`) with Repository & Unit of Work patterns.

---

## 👤 Author

**S. M. MARZANUL HOQUE**
- Email: [marzanulru17@gmail.com](mailto:marzanulru17@gmail.com)
- GitHub: [@marzanulhoque](https://github.com/marzanulhoque)

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
