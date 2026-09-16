using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotnetProject.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase2ExpandedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2026, 9, 9, 12, 0, 5, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Message", "Timestamp" },
                values: new object[] { "Test run passed! 16 tests passed, 0 failed, 0 skipped in 1.05 seconds.", new DateTime(2026, 9, 9, 12, 0, 18, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 3,
                column: "Timestamp",
                value: new DateTime(2026, 9, 9, 12, 0, 28, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 4,
                column: "Timestamp",
                value: new DateTime(2026, 9, 9, 12, 0, 40, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 5,
                column: "Timestamp",
                value: new DateTime(2026, 9, 9, 12, 0, 48, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 1,
                column: "DeployedAt",
                value: new DateTime(2026, 9, 9, 12, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 2,
                column: "DeployedAt",
                value: new DateTime(2026, 9, 10, 16, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AppName", "DeployedAt", "DeployedBy" },
                values: new object[] { "Payment & Billing Gateway", new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "FinTech SRE" });

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 4,
                column: "DeployedAt",
                value: new DateTime(2026, 9, 12, 4, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Deployments",
                columns: new[] { "Id", "AppName", "CommitHash", "DeployedAt", "DeployedBy", "Environment", "ExecutionDurationSeconds", "Notes", "ProjectId", "Status", "Version" },
                values: new object[,]
                {
                    { 5, "DevOps Release Tracker", "18e0827", new DateTime(2026, 9, 12, 11, 0, 0, 0, DateTimeKind.Utc), "GitHub Actions [CI/CD]", "Production", 38, "Phase 2 True 3-Tier Architecture live on AWS EC2 with systemd sandboxing.", 1, "Successful", "v2.0.0" },
                    { 10, "Payment & Billing Gateway", "5b3e901", new DateTime(2026, 9, 12, 15, 20, 0, 0, DateTimeKind.Utc), "Release Engineer", "Staging", 36, "Staging smoke tests passed for payment gateway failover.", 2, "Successful", "v2.3.2-patch" }
                });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 29, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Name", "RepositoryUrl", "Slug" },
                values: new object[] { new DateTime(2026, 8, 31, 10, 0, 0, 0, DateTimeKind.Utc), "Payment & Billing Gateway", "https://github.com/enterprise/payment-gateway", "payment-gateway" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "RepositoryUrl" },
                values: new object[] { new DateTime(2026, 9, 2, 10, 0, 0, 0, DateTimeKind.Utc), "https://github.com/enterprise/notification-worker" });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedAt", "Name", "Owner", "RepositoryUrl", "Slug", "TechStack" },
                values: new object[,]
                {
                    { 4, new DateTime(2026, 9, 5, 10, 0, 0, 0, DateTimeKind.Utc), "Auth & Identity Provider", "Security Core", "https://github.com/enterprise/auth-service", "auth-identity", "ASP.NET Core 8.0, OIDC, OpenIddict" },
                    { 5, new DateTime(2026, 9, 7, 10, 0, 0, 0, DateTimeKind.Utc), "Telemetry & Metrics Collector", "Observability SRE", "https://github.com/enterprise/telemetry-agent", "telemetry-collector", "OpenTelemetry, Prometheus, Grafana" },
                    { 6, new DateTime(2026, 9, 8, 10, 0, 0, 0, DateTimeKind.Utc), "Cloud Infrastructure & IaC Engine", "Cloud Operations", "https://github.com/enterprise/cloud-terraform", "cloud-iac-engine", "Terraform, AWS, Ansible, GitHub Actions" },
                    { 7, new DateTime(2026, 9, 10, 10, 0, 0, 0, DateTimeKind.Utc), "Customer Web Experience Portal", "Frontend Experience", "https://github.com/enterprise/customer-web", "customer-portal", "React 19, TypeScript, Vite, Tailwind CSS" }
                });

            migrationBuilder.InsertData(
                table: "DeploymentLogs",
                columns: new[] { "Id", "DeploymentRecordId", "LogLevel", "Message", "StepName", "Timestamp" },
                values: new object[,]
                {
                    { 6, 5, "Info", "EF Core Migration applied to MySQL 8.0 disk storage (__EFMigrationsHistory updated).", "Database Migration", new DateTime(2026, 9, 12, 11, 0, 10, 0, DateTimeKind.Utc) },
                    { 7, 5, "Info", "DynamicUser=yes and ProtectSystem=strict verified active in kernel cgroup.", "systemd Sandboxing Verification", new DateTime(2026, 9, 12, 11, 0, 22, 0, DateTimeKind.Utc) },
                    { 8, 5, "Info", "Reverse proxy routes verified: / -> Kestrel :5000, /api/ -> Kestrel :5050 (HTTP 200 OK).", "Nginx Proxy Upstream Check", new DateTime(2026, 9, 12, 11, 0, 38, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Deployments",
                columns: new[] { "Id", "AppName", "CommitHash", "DeployedAt", "DeployedBy", "Environment", "ExecutionDurationSeconds", "Notes", "ProjectId", "Status", "Version" },
                values: new object[,]
                {
                    { 6, "Auth & Identity Provider", "a9b8c7d", new DateTime(2026, 9, 12, 12, 15, 0, 0, DateTimeKind.Utc), "Security Automation", "Production", 44, "OAuth2 refresh token rotation and rate limiting enabled.", 4, "Successful", "v1.2.0" },
                    { 7, "Telemetry & Metrics Collector", "7e6d5c4", new DateTime(2026, 9, 12, 13, 40, 0, 0, DateTimeKind.Utc), "SRE Pipeline", "Production", 29, "Scraping Kestrel endpoints at /api/health with 15s interval.", 5, "Successful", "v1.0.4" },
                    { 8, "Cloud Infrastructure & IaC Engine", "c1e8b93", new DateTime(2026, 9, 12, 14, 10, 0, 0, DateTimeKind.Utc), "Terraform Cloud", "Production", 64, "AWS EC2 security group ingress tightening and VPC peering verification.", 6, "Successful", "v2.1.0" },
                    { 9, "Customer Web Experience Portal", "2d4f8a1", new DateTime(2026, 9, 12, 14, 50, 0, 0, DateTimeKind.Utc), "Frontend CI Bot", "Production", 31, "React 19 SPA asset bundle CDN cache invalidation and deploy.", 7, "Successful", "v3.4.0" }
                });

            migrationBuilder.InsertData(
                table: "DeploymentLogs",
                columns: new[] { "Id", "DeploymentRecordId", "LogLevel", "Message", "StepName", "Timestamp" },
                values: new object[] { 9, 8, "Info", "Terraform applied 4 resources: aws_security_group_rule, aws_route_table. 0 failed.", "Terraform Apply", new DateTime(2026, 9, 12, 14, 10, 55, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2026, 8, 31, 16, 0, 5, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Message", "Timestamp" },
                values: new object[] { "Test run passed! 8 tests passed, 0 failed, 0 skipped in 1.05 seconds.", new DateTime(2026, 8, 31, 16, 0, 18, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 3,
                column: "Timestamp",
                value: new DateTime(2026, 8, 31, 16, 0, 28, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 4,
                column: "Timestamp",
                value: new DateTime(2026, 8, 31, 16, 0, 40, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "DeploymentLogs",
                keyColumn: "Id",
                keyValue: 5,
                column: "Timestamp",
                value: new DateTime(2026, 8, 31, 16, 0, 48, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 1,
                column: "DeployedAt",
                value: new DateTime(2026, 8, 31, 16, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 2,
                column: "DeployedAt",
                value: new DateTime(2026, 9, 1, 4, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AppName", "DeployedAt", "DeployedBy" },
                values: new object[] { "Payment & Billing Service", new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Utc), "Platform Engineer" });

            migrationBuilder.UpdateData(
                table: "Deployments",
                keyColumn: "Id",
                keyValue: 4,
                column: "DeployedAt",
                value: new DateTime(2026, 9, 1, 9, 30, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 22, 10, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Name", "RepositoryUrl", "Slug" },
                values: new object[] { new DateTime(2026, 8, 24, 10, 0, 0, 0, DateTimeKind.Utc), "Payment & Billing Service", "https://github.com/company/payment-service", "payment-service" });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "RepositoryUrl" },
                values: new object[] { new DateTime(2026, 8, 27, 10, 0, 0, 0, DateTimeKind.Utc), "https://github.com/company/notification-worker" });
        }
    }
}
