# 🚀 DevOps Deployment Tracker — ASP.NET Core 8.0 on AWS EC2

[![CI/CD Build & Test](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions/workflows/deploy.yml/badge.svg)](https://github.com/marzanulhoque/dotnet-devops-release-tracker/actions)
[![Live Demo](https://img.shields.io/badge/Live%20Demo-13.220.86.134-success?logo=amazonaws)](http://13.220.86.134/)
[![Health Status](https://img.shields.io/badge/Health%20Check-Healthy%20200%20OK-brightgreen?logo=statuspage)](http://13.220.86.134/health)
![.NET 8.0](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4?logo=dotnet)
![AWS EC2](https://img.shields.io/badge/AWS-EC2%20Linux-FF9900?logo=amazonec2)
![MySQL](https://img.shields.io/badge/Database-MySQL%208.0-4479A1?logo=mysql)
![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?logo=githubactions)
![systemd](https://img.shields.io/badge/Daemon-systemd-black?logo=linux)

> 🌐 **Live Public Deployment:** [http://13.220.86.134/](http://13.220.86.134/)  
> 🩺 **Live Health & Observability:** [http://13.220.86.134/health](http://13.220.86.134/health)  
> ⚙️ **Host Architecture:** AWS EC2 Linux (Ubuntu 24.04 LTS) • Nginx Reverse Proxy (:80) • systemd Daemon (`dotnetapp.service`) • Kestrel (:5000)

A production-grade **ASP.NET Core 8.0 MVC** application automated with **GitHub Actions CI/CD** targeting an **AWS Linux EC2** instance running native Kestrel managed by **`systemd`** and reverse-proxied with **Nginx** (no container overhead). 

This project serves as **Phase 1** of an architectural evolution journey from a monolithic MVC application to an enterprise **3-Tier Architecture** (Presentation $\rightarrow$ REST Web API $\rightarrow$ Data Access Layer).

---

## 🏗️ Architecture & Continuous Delivery Pipeline

```
[ Developer ]
      │  git push origin main
      ▼
[ GitHub Actions CI/CD Pipeline ]
      ├── 1. Setup .NET 8.0 SDK
      ├── 2. dotnet restore & dotnet build (Release)
      ├── 3. Execute Automated xUnit Test Suite (Fail-Safe Gate)
      ├── 4. dotnet publish -c Release
      └── 5. Secure Artifact Transfer to AWS EC2 (SSH/SCP)
            │
            ▼
[ AWS EC2 Linux (Ubuntu / Amazon Linux) ]
      ├── [ Nginx Reverse Proxy (:80 / :443) ]
      │           │  proxy_pass (http://127.0.0.1:5000)
      │           ▼
      ├── [ Kestrel Server (systemd daemon: dotnetapp.service) ]
      │           │
      │           ▼
      └── [ MySQL 8.0 Database ] (Pomelo EF Core Provider)
```

---

## ✨ Key Features

- **Live Cloud Deployment:** Deployed and actively serving traffic on AWS EC2 at [http://13.220.86.134/](http://13.220.86.134/) with real-time health metrics at [`/health`](http://13.220.86.134/health).
- **DevOps Release Dashboard:** Track, log, and filter deployment records across environments (`Development`, `Staging`, `Production`) with commit hashes, deployer info, and release notes.
- **Automated Fail-Safe CI Gates:** Every push runs automated xUnit tests. If model validation, controller logic, or health checks fail, the deployment is aborted immediately.
- **Zero-Container EC2 Deployment:** Direct execution via Kestrel on AWS EC2 Free Tier (`t2.micro` / `t3.micro`), delivering maximum throughput and minimal cloud cost.
- **Process Supervision with Linux `systemd`:** Automated daemon recovery on crashes (`RestartSec=10`), background execution, and centralized logs via `journalctl`.
- **Nginx Reverse Proxy:** Terminates HTTP traffic, enables gzip compression, and manages forwarded headers (`X-Forwarded-For`, `X-Forwarded-Proto`).
- **Observability Endpoint (`/health`):** Returns JSON diagnostics, server uptime, environment name, and live MySQL connectivity status.
- **Secure Secret Separation:** Zero database passwords or secrets committed to git. Credentials are read from server-only environment files (`/etc/dotnetapp.env`) or AWS Parameter Store.

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

- [x] **Phase 1: ASP.NET Core 8.0 MVC Monolith (Current)**
  - Direct MySQL EF Core mapping, Razor dashboard, xUnit test gates, and GitHub Actions CI/CD to AWS EC2.
- [ ] **Phase 2: Decoupled REST Web API Tier (Upcoming)**
  - Extracting business logic into a dedicated REST API service project (`DotnetProject.Api`) with CI/CD webhook automation.
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
