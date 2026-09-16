using Microsoft.EntityFrameworkCore;
using DotnetProject.Core.Entities;
using System;

namespace DotnetProject.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<DeploymentRecord> Deployments => Set<DeploymentRecord>();
        public DbSet<DeploymentLog> DeploymentLogs => Set<DeploymentLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Project entity
            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Slug).IsRequired().HasMaxLength(50);
                entity.Property(p => p.RepositoryUrl).HasMaxLength(255);
                entity.Property(p => p.Owner).HasMaxLength(100);
                entity.Property(p => p.TechStack).HasMaxLength(100);
            });

            // Configure DeploymentRecord entity
            modelBuilder.Entity<DeploymentRecord>(entity =>
            {
                entity.ToTable("Deployments");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.AppName).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Version).IsRequired().HasMaxLength(50);
                entity.Property(d => d.Environment).IsRequired().HasMaxLength(50);
                entity.Property(d => d.Status).IsRequired().HasMaxLength(50);
                entity.Property(d => d.CommitHash).HasMaxLength(40);
                entity.Property(d => d.DeployedBy).HasMaxLength(100);

                // Relationship: Project 1-to-many Deployments
                entity.HasOne(d => d.Project)
                      .WithMany(p => p.Deployments)
                      .HasForeignKey(d => d.ProjectId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure DeploymentLog entity
            modelBuilder.Entity<DeploymentLog>(entity =>
            {
                entity.ToTable("DeploymentLogs");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.StepName).IsRequired().HasMaxLength(100);
                entity.Property(l => l.LogLevel).IsRequired().HasMaxLength(20);
                entity.Property(l => l.Message).IsRequired().HasMaxLength(1000);

                // Relationship: DeploymentRecord 1-to-many DeploymentLogs
                entity.HasOne(l => l.DeploymentRecord)
                      .WithMany(d => d.Logs)
                      .HasForeignKey(l => l.DeploymentRecordId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Pre-seed realistic initial DevOps data
            SeedInitialData(modelBuilder);
        }

        private static void SeedInitialData(ModelBuilder modelBuilder)
        {
            var baseTime = new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc);

            // 1. Seed Projects (7 Modern Microservices & Infrastructure Projects)
            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    Id = 1,
                    Name = "DevOps Release Tracker",
                    Slug = "release-tracker",
                    RepositoryUrl = "https://github.com/MarzanulHoque/dotnet-devops-release-tracker",
                    Owner = "Platform Engineering",
                    TechStack = "ASP.NET Core 8.0, Nginx, systemd, MySQL",
                    CreatedAt = baseTime.AddDays(-14)
                },
                new Project
                {
                    Id = 2,
                    Name = "Payment & Billing Gateway",
                    Slug = "payment-gateway",
                    RepositoryUrl = "https://github.com/enterprise/payment-gateway",
                    Owner = "FinTech Core",
                    TechStack = ".NET 8 Web API, PostgreSQL, Stripe",
                    CreatedAt = baseTime.AddDays(-12)
                },
                new Project
                {
                    Id = 3,
                    Name = "Notification Dispatcher Daemon",
                    Slug = "notification-daemon",
                    RepositoryUrl = "https://github.com/enterprise/notification-worker",
                    Owner = "Messaging Infrastructure",
                    TechStack = "Go 1.22, RabbitMQ, Redis",
                    CreatedAt = baseTime.AddDays(-10)
                },
                new Project
                {
                    Id = 4,
                    Name = "Auth & Identity Provider",
                    Slug = "auth-identity",
                    RepositoryUrl = "https://github.com/enterprise/auth-service",
                    Owner = "Security Core",
                    TechStack = "ASP.NET Core 8.0, OIDC, OpenIddict",
                    CreatedAt = baseTime.AddDays(-7)
                },
                new Project
                {
                    Id = 5,
                    Name = "Telemetry & Metrics Collector",
                    Slug = "telemetry-collector",
                    RepositoryUrl = "https://github.com/enterprise/telemetry-agent",
                    Owner = "Observability SRE",
                    TechStack = "OpenTelemetry, Prometheus, Grafana",
                    CreatedAt = baseTime.AddDays(-5)
                },
                new Project
                {
                    Id = 6,
                    Name = "Cloud Infrastructure & IaC Engine",
                    Slug = "cloud-iac-engine",
                    RepositoryUrl = "https://github.com/enterprise/cloud-terraform",
                    Owner = "Cloud Operations",
                    TechStack = "Terraform, AWS, Ansible, GitHub Actions",
                    CreatedAt = baseTime.AddDays(-4)
                },
                new Project
                {
                    Id = 7,
                    Name = "Customer Web Experience Portal",
                    Slug = "customer-portal",
                    RepositoryUrl = "https://github.com/enterprise/customer-web",
                    Owner = "Frontend Experience",
                    TechStack = "React 19, TypeScript, Vite, Tailwind CSS",
                    CreatedAt = baseTime.AddDays(-2)
                }
            );

            // 2. Seed Deployment Records (Rich production, staging, and dev releases)
            modelBuilder.Entity<DeploymentRecord>().HasData(
                new DeploymentRecord
                {
                    Id = 1,
                    ProjectId = 1,
                    AppName = "DevOps Release Tracker",
                    Version = "v1.0.0",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "GitHub Actions CI/CD",
                    CommitHash = "bc8d5e0",
                    DeployedAt = baseTime.AddDays(-3).AddHours(2),
                    ExecutionDurationSeconds = 48,
                    Notes = "Initial production deployment to AWS EC2 via systemd daemon and Nginx reverse proxy."
                },
                new DeploymentRecord
                {
                    Id = 2,
                    ProjectId = 1,
                    AppName = "DevOps Release Tracker",
                    Version = "v1.1.0-preview",
                    Environment = "Staging",
                    Status = "Successful",
                    DeployedBy = "GitHub Actions CI/CD",
                    CommitHash = "93bc198",
                    DeployedAt = baseTime.AddDays(-2).AddHours(6),
                    ExecutionDurationSeconds = 42,
                    Notes = "Staging verification for decoupled REST API endpoints and Swagger UI."
                },
                new DeploymentRecord
                {
                    Id = 3,
                    ProjectId = 2,
                    AppName = "Payment & Billing Gateway",
                    Version = "v2.3.1",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "FinTech SRE",
                    CommitHash = "f4c9a82",
                    DeployedAt = baseTime.AddDays(-1).AddHours(14),
                    ExecutionDurationSeconds = 55,
                    Notes = "Stripe webhook retry exponential backoff optimization."
                },
                new DeploymentRecord
                {
                    Id = 4,
                    ProjectId = 3,
                    AppName = "Notification Dispatcher Daemon",
                    Version = "v0.9.4",
                    Environment = "Development",
                    Status = "In-Progress",
                    DeployedBy = "Developer Local Runner",
                    CommitHash = "3e28b10",
                    DeployedAt = baseTime.AddDays(-1).AddHours(18),
                    ExecutionDurationSeconds = 20,
                    Notes = "Testing RabbitMQ dead-letter exchange consumers."
                },
                new DeploymentRecord
                {
                    Id = 5,
                    ProjectId = 1,
                    AppName = "DevOps Release Tracker",
                    Version = "v2.0.0",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "GitHub Actions [CI/CD]",
                    CommitHash = "18e0827",
                    DeployedAt = baseTime.AddHours(1),
                    ExecutionDurationSeconds = 38,
                    Notes = "Phase 2 True 3-Tier Architecture live on AWS EC2 with systemd sandboxing."
                },
                new DeploymentRecord
                {
                    Id = 6,
                    ProjectId = 4,
                    AppName = "Auth & Identity Provider",
                    Version = "v1.2.0",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "Security Automation",
                    CommitHash = "a9b8c7d",
                    DeployedAt = baseTime.AddHours(2).AddMinutes(15),
                    ExecutionDurationSeconds = 44,
                    Notes = "OAuth2 refresh token rotation and rate limiting enabled."
                },
                new DeploymentRecord
                {
                    Id = 7,
                    ProjectId = 5,
                    AppName = "Telemetry & Metrics Collector",
                    Version = "v1.0.4",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "SRE Pipeline",
                    CommitHash = "7e6d5c4",
                    DeployedAt = baseTime.AddHours(3).AddMinutes(40),
                    ExecutionDurationSeconds = 29,
                    Notes = "Scraping Kestrel endpoints at /api/health with 15s interval."
                },
                new DeploymentRecord
                {
                    Id = 8,
                    ProjectId = 6,
                    AppName = "Cloud Infrastructure & IaC Engine",
                    Version = "v2.1.0",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "Terraform Cloud",
                    CommitHash = "c1e8b93",
                    DeployedAt = baseTime.AddHours(4).AddMinutes(10),
                    ExecutionDurationSeconds = 64,
                    Notes = "AWS EC2 security group ingress tightening and VPC peering verification."
                },
                new DeploymentRecord
                {
                    Id = 9,
                    ProjectId = 7,
                    AppName = "Customer Web Experience Portal",
                    Version = "v3.4.0",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "Frontend CI Bot",
                    CommitHash = "2d4f8a1",
                    DeployedAt = baseTime.AddHours(4).AddMinutes(50),
                    ExecutionDurationSeconds = 31,
                    Notes = "React 19 SPA asset bundle CDN cache invalidation and deploy."
                },
                new DeploymentRecord
                {
                    Id = 10,
                    ProjectId = 2,
                    AppName = "Payment & Billing Gateway",
                    Version = "v2.3.2-patch",
                    Environment = "Staging",
                    Status = "Successful",
                    DeployedBy = "Release Engineer",
                    CommitHash = "5b3e901",
                    DeployedAt = baseTime.AddHours(5).AddMinutes(20),
                    ExecutionDurationSeconds = 36,
                    Notes = "Staging smoke tests passed for payment gateway failover."
                }
            );

            // 3. Seed Pipeline Execution Logs
            modelBuilder.Entity<DeploymentLog>().HasData(
                new DeploymentLog
                {
                    Id = 1,
                    DeploymentRecordId = 1,
                    StepName = "Restore Dependencies",
                    LogLevel = "Info",
                    Message = "dotnet restore completed in 1.82 seconds with 0 warnings.",
                    Timestamp = baseTime.AddDays(-3).AddHours(2).AddSeconds(5)
                },
                new DeploymentLog
                {
                    Id = 2,
                    DeploymentRecordId = 1,
                    StepName = "Automated xUnit Test Gate",
                    LogLevel = "Info",
                    Message = "Test run passed! 16 tests passed, 0 failed, 0 skipped in 1.05 seconds.",
                    Timestamp = baseTime.AddDays(-3).AddHours(2).AddSeconds(18)
                },
                new DeploymentLog
                {
                    Id = 3,
                    DeploymentRecordId = 1,
                    StepName = "Publish Artifacts",
                    LogLevel = "Info",
                    Message = "Release binaries compiled and published to ./publish output directory.",
                    Timestamp = baseTime.AddDays(-3).AddHours(2).AddSeconds(28)
                },
                new DeploymentLog
                {
                    Id = 4,
                    DeploymentRecordId = 1,
                    StepName = "SCP Transfer",
                    LogLevel = "Info",
                    Message = "Binaries transferred via encrypted SCP to /var/www/dotnetapp on AWS EC2.",
                    Timestamp = baseTime.AddDays(-3).AddHours(2).AddSeconds(40)
                },
                new DeploymentLog
                {
                    Id = 5,
                    DeploymentRecordId = 1,
                    StepName = "systemd Service Restart",
                    LogLevel = "Info",
                    Message = "sudo systemctl restart dotnetapp.service executed cleanly. Daemon active (running).",
                    Timestamp = baseTime.AddDays(-3).AddHours(2).AddSeconds(48)
                },
                new DeploymentLog
                {
                    Id = 6,
                    DeploymentRecordId = 5,
                    StepName = "Database Migration",
                    LogLevel = "Info",
                    Message = "EF Core Migration applied to MySQL 8.0 disk storage (__EFMigrationsHistory updated).",
                    Timestamp = baseTime.AddHours(1).AddSeconds(10)
                },
                new DeploymentLog
                {
                    Id = 7,
                    DeploymentRecordId = 5,
                    StepName = "systemd Sandboxing Verification",
                    LogLevel = "Info",
                    Message = "DynamicUser=yes and ProtectSystem=strict verified active in kernel cgroup.",
                    Timestamp = baseTime.AddHours(1).AddSeconds(22)
                },
                new DeploymentLog
                {
                    Id = 8,
                    DeploymentRecordId = 5,
                    StepName = "Nginx Proxy Upstream Check",
                    LogLevel = "Info",
                    Message = "Reverse proxy routes verified: / -> Kestrel :5000, /api/ -> Kestrel :5050 (HTTP 200 OK).",
                    Timestamp = baseTime.AddHours(1).AddSeconds(38)
                },
                new DeploymentLog
                {
                    Id = 9,
                    DeploymentRecordId = 8,
                    StepName = "Terraform Apply",
                    LogLevel = "Info",
                    Message = "Terraform applied 4 resources: aws_security_group_rule, aws_route_table. 0 failed.",
                    Timestamp = baseTime.AddHours(4).AddMinutes(10).AddSeconds(55)
                }
            );
        }
    }
}
