using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotnetProject.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialPhase2Migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RepositoryUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Owner = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TechStack = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Deployments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    AppName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Version = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Environment = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeployedBy = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommitHash = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeployedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExecutionDurationSeconds = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deployments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeploymentLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DeploymentRecordId = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogLevel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeploymentLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeploymentLogs_Deployments_DeploymentRecordId",
                        column: x => x.DeploymentRecordId,
                        principalTable: "Deployments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedAt", "Name", "Owner", "RepositoryUrl", "Slug", "TechStack" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 22, 10, 0, 0, 0, DateTimeKind.Utc), "DevOps Release Tracker", "Platform Engineering", "https://github.com/MarzanulHoque/dotnet-devops-release-tracker", "release-tracker", "ASP.NET Core 8.0, Nginx, systemd, MySQL" },
                    { 2, new DateTime(2026, 8, 24, 10, 0, 0, 0, DateTimeKind.Utc), "Payment & Billing Service", "FinTech Core", "https://github.com/company/payment-service", "payment-service", ".NET 8 Web API, PostgreSQL, Stripe" },
                    { 3, new DateTime(2026, 8, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Notification Dispatcher Daemon", "Messaging Infrastructure", "https://github.com/company/notification-worker", "notification-daemon", "Go 1.22, RabbitMQ, Redis" }
                });

            migrationBuilder.InsertData(
                table: "Deployments",
                columns: new[] { "Id", "AppName", "CommitHash", "DeployedAt", "DeployedBy", "Environment", "ExecutionDurationSeconds", "Notes", "ProjectId", "Status", "Version" },
                values: new object[,]
                {
                    { 1, "DevOps Release Tracker", "bc8d5e0", new DateTime(2026, 8, 31, 16, 0, 0, 0, DateTimeKind.Utc), "GitHub Actions CI/CD", "Production", 48, "Initial production deployment to AWS EC2 via systemd daemon and Nginx reverse proxy.", 1, "Successful", "v1.0.0" },
                    { 2, "DevOps Release Tracker", "93bc198", new DateTime(2026, 9, 1, 4, 0, 0, 0, DateTimeKind.Utc), "GitHub Actions CI/CD", "Staging", 42, "Staging verification for decoupled REST API endpoints and Swagger UI.", 1, "Successful", "v1.1.0-preview" },
                    { 3, "Payment & Billing Service", "f4c9a82", new DateTime(2026, 9, 1, 8, 0, 0, 0, DateTimeKind.Utc), "Platform Engineer", "Production", 55, "Stripe webhook retry exponential backoff optimization.", 2, "Successful", "v2.3.1" },
                    { 4, "Notification Dispatcher Daemon", "3e28b10", new DateTime(2026, 9, 1, 9, 30, 0, 0, DateTimeKind.Utc), "Developer Local Runner", "Development", 20, "Testing RabbitMQ dead-letter exchange consumers.", 3, "In-Progress", "v0.9.4" }
                });

            migrationBuilder.InsertData(
                table: "DeploymentLogs",
                columns: new[] { "Id", "DeploymentRecordId", "LogLevel", "Message", "StepName", "Timestamp" },
                values: new object[,]
                {
                    { 1, 1, "Info", "dotnet restore completed in 1.82 seconds with 0 warnings.", "Restore Dependencies", new DateTime(2026, 8, 31, 16, 0, 5, 0, DateTimeKind.Utc) },
                    { 2, 1, "Info", "Test run passed! 8 tests passed, 0 failed, 0 skipped in 1.05 seconds.", "Automated xUnit Test Gate", new DateTime(2026, 8, 31, 16, 0, 18, 0, DateTimeKind.Utc) },
                    { 3, 1, "Info", "Release binaries compiled and published to ./publish output directory.", "Publish Artifacts", new DateTime(2026, 8, 31, 16, 0, 28, 0, DateTimeKind.Utc) },
                    { 4, 1, "Info", "Binaries transferred via encrypted SCP to /var/www/dotnetapp on AWS EC2.", "SCP Transfer", new DateTime(2026, 8, 31, 16, 0, 40, 0, DateTimeKind.Utc) },
                    { 5, 1, "Info", "sudo systemctl restart dotnetapp.service executed cleanly. Daemon active (running).", "systemd Service Restart", new DateTime(2026, 8, 31, 16, 0, 48, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeploymentLogs_DeploymentRecordId",
                table: "DeploymentLogs",
                column: "DeploymentRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_ProjectId",
                table: "Deployments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Slug",
                table: "Projects",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeploymentLogs");

            migrationBuilder.DropTable(
                name: "Deployments");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
