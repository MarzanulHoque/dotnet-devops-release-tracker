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
            var seedTime = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc);

            // 1. Seed Projects
            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    Id = 1,
                    Name = "DevOps Release Tracker",
                    Slug = "release-tracker",
                    RepositoryUrl = "https://github.com/MarzanulHoque/dotnet-devops-release-tracker",
                    Owner = "Platform Engineering",
                    TechStack = "ASP.NET Core 8.0, Nginx, systemd, MySQL",
                    CreatedAt = seedTime.AddDays(-10)
                },
                new Project
                {
                    Id = 2,
                    Name = "Payment & Billing Service",
                    Slug = "payment-service",
                    RepositoryUrl = "https://github.com/company/payment-service",
                    Owner = "FinTech Core",
                    TechStack = ".NET 8 Web API, PostgreSQL, Stripe",
                    CreatedAt = seedTime.AddDays(-8)
                },
                new Project
                {
                    Id = 3,
                    Name = "Notification Dispatcher Daemon",
                    Slug = "notification-daemon",
                    RepositoryUrl = "https://github.com/company/notification-worker",
                    Owner = "Messaging Infrastructure",
                    TechStack = "Go 1.22, RabbitMQ, Redis",
                    CreatedAt = seedTime.AddDays(-5)
                }
            );

            // 2. Seed Deployment Records
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
                    DeployedAt = seedTime.AddHours(-18),
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
                    DeployedAt = seedTime.AddHours(-6),
                    ExecutionDurationSeconds = 42,
                    Notes = "Staging verification for decoupled REST API endpoints and Swagger UI."
                },
                new DeploymentRecord
                {
                    Id = 3,
                    ProjectId = 2,
                    AppName = "Payment & Billing Service",
                    Version = "v2.3.1",
                    Environment = "Production",
                    Status = "Successful",
                    DeployedBy = "Platform Engineer",
                    CommitHash = "f4c9a82",
                    DeployedAt = seedTime.AddHours(-2),
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
                    DeployedAt = seedTime.AddMinutes(-30),
                    ExecutionDurationSeconds = 20,
                    Notes = "Testing RabbitMQ dead-letter exchange consumers."
                }
            );

            // 3. Seed Pipeline Execution Logs for Deployment #1
            modelBuilder.Entity<DeploymentLog>().HasData(
                new DeploymentLog
                {
                    Id = 1,
                    DeploymentRecordId = 1,
                    StepName = "Restore Dependencies",
                    LogLevel = "Info",
                    Message = "dotnet restore completed in 1.82 seconds with 0 warnings.",
                    Timestamp = seedTime.AddHours(-18).AddSeconds(5)
                },
                new DeploymentLog
                {
                    Id = 2,
                    DeploymentRecordId = 1,
                    StepName = "Automated xUnit Test Gate",
                    LogLevel = "Info",
                    Message = "Test run passed! 8 tests passed, 0 failed, 0 skipped in 1.05 seconds.",
                    Timestamp = seedTime.AddHours(-18).AddSeconds(18)
                },
                new DeploymentLog
                {
                    Id = 3,
                    DeploymentRecordId = 1,
                    StepName = "Publish Artifacts",
                    LogLevel = "Info",
                    Message = "Release binaries compiled and published to ./publish output directory.",
                    Timestamp = seedTime.AddHours(-18).AddSeconds(28)
                },
                new DeploymentLog
                {
                    Id = 4,
                    DeploymentRecordId = 1,
                    StepName = "SCP Transfer",
                    LogLevel = "Info",
                    Message = "Binaries transferred via encrypted SCP to /var/www/dotnetapp on AWS EC2.",
                    Timestamp = seedTime.AddHours(-18).AddSeconds(40)
                },
                new DeploymentLog
                {
                    Id = 5,
                    DeploymentRecordId = 1,
                    StepName = "systemd Service Restart",
                    LogLevel = "Info",
                    Message = "sudo systemctl restart dotnetapp.service executed cleanly. Daemon active (running).",
                    Timestamp = seedTime.AddHours(-18).AddSeconds(48)
                }
            );
        }
    }
}
