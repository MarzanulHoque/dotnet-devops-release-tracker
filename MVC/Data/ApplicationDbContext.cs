using Microsoft.EntityFrameworkCore;
using DotnetProject.Web.Models;
using System;

namespace DotnetProject.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DeploymentRecord> Deployments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeploymentRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Environment);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.DeployedAt);

                // Initial Seed Data for immediate dashboard presentation
                entity.HasData(
                    new DeploymentRecord
                    {
                        Id = 1,
                        AppName = "DotnetProject.Web",
                        Version = "v1.0.0",
                        Environment = "Production",
                        Status = "Successful",
                        DeployedBy = "GitHub Actions CI/CD",
                        CommitHash = "init001",
                        DeployedAt = new DateTime(2026, 9, 1, 10, 30, 0, DateTimeKind.Utc),
                        Notes = "Initial production deployment on AWS Linux EC2 via Kestrel systemd"
                    },
                    new DeploymentRecord
                    {
                        Id = 2,
                        AppName = "PaymentService.Worker",
                        Version = "v1.4.2",
                        Environment = "Staging",
                        Status = "Successful",
                        DeployedBy = "GitHub Actions CI/CD",
                        CommitHash = "b84f2c9",
                        DeployedAt = new DateTime(2026, 9, 3, 14, 15, 0, DateTimeKind.Utc),
                        Notes = "Kafka consumer optimization and retry backoff update"
                    },
                    new DeploymentRecord
                    {
                        Id = 3,
                        AppName = "AuthService.Api",
                        Version = "v2.1.0",
                        Environment = "Development",
                        Status = "Successful",
                        DeployedBy = "DevOps Engineer",
                        CommitHash = "9d3e8a4",
                        DeployedAt = new DateTime(2026, 9, 5, 09, 00, 0, DateTimeKind.Utc),
                        Notes = "OAuth2 / OIDC token rotation integration testing"
                    }
                );
            });
        }
    }
}
