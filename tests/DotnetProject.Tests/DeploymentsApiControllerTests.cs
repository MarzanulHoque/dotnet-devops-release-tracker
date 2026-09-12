using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetProject.Api.Controllers;
using DotnetProject.Core.Data;
using DotnetProject.Core.DTOs;
using DotnetProject.Core.Entities;
using Xunit;

namespace DotnetProject.Tests
{
    public class DeploymentsApiControllerTests
    {
        private ApplicationDbContext CreateInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithSeededDeployments()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_GetAll");
            var controller = new DeploymentsController(context);

            // Act
            var actionResult = await controller.GetAll(null, null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var records = Assert.IsAssignableFrom<IEnumerable<DeploymentResponseDto>>(okResult.Value);
            Assert.NotEmpty(records);
        }

        [Fact]
        public async Task GetAll_FiltersByEnvironment_Correctly()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_FilterEnv");
            var controller = new DeploymentsController(context);

            // Act
            var actionResult = await controller.GetAll("Production", null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var records = Assert.IsAssignableFrom<IEnumerable<DeploymentResponseDto>>(okResult.Value);
            Assert.All(records, r => Assert.Equal("Production", r.Environment));
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_GetById_NotFound");
            var controller = new DeploymentsController(context);

            // Act
            var actionResult = await controller.GetById(99999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenValid()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_Create");
            var controller = new DeploymentsController(context);

            var dto = new CreateDeploymentDto
            {
                AppName = "BillingMicroservice",
                Version = "v3.0.0",
                Environment = "Production",
                Status = "Successful",
                DeployedBy = "Platform Engineer",
                CommitHash = "c0ffee1",
                Notes = "Automated test deployment"
            };

            // Act
            var actionResult = await controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdRecord = Assert.IsType<DeploymentResponseDto>(createdResult.Value);
            Assert.Equal("BillingMicroservice", createdRecord.AppName);
            Assert.Equal("v3.0.0", createdRecord.Version);
            Assert.True(createdRecord.Id > 0);
        }

        [Fact]
        public async Task Update_ModifiesStatus_AndReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_Update");
            var controller = new DeploymentsController(context);

            var updateDto = new UpdateDeploymentDto
            {
                Status = "Failed",
                Notes = "Health check timed out during rollout."
            };

            // Act
            var result = await controller.Update(1, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updated = await context.Deployments.FindAsync(1);
            Assert.NotNull(updated);
            Assert.Equal("Failed", updated.Status);
            Assert.Equal("Health check timed out during rollout.", updated.Notes);
        }

        [Fact]
        public async Task Delete_RemovesDeployment_AndReturnsNoContent()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_Delete");
            var controller = new DeploymentsController(context);

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var deleted = await context.Deployments.FindAsync(1);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Webhook_CreatesDeployment_AndLogsPipelineEvent()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_Webhook");
            var controller = new DeploymentsController(context);

            var payload = new CiWebhookPayloadDto
            {
                Repository = "dotnet-devops-release-tracker",
                ReleaseVersion = "v2.0.0-rc1",
                Environment = "Production",
                Status = "Successful",
                CommitSha = "7a8b9c0",
                Actor = "github-actions[bot]",
                Summary = "Automated push from GitHub Actions runner"
            };

            // Act
            var actionResult = await controller.Webhook(payload);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var response = Assert.IsType<DeploymentResponseDto>(createdResult.Value);
            Assert.Equal("dotnet-devops-release-tracker", response.AppName);
            Assert.Equal("v2.0.0-rc1", response.Version);
            Assert.Equal(1, response.LogCount);

            // Verify the audit log was persisted
            var log = await context.DeploymentLogs.FirstOrDefaultAsync(l => l.DeploymentRecordId == response.Id);
            Assert.NotNull(log);
            Assert.Equal("CI/CD Webhook Ingestion", log.StepName);
        }

        [Fact]
        public async Task AddLog_AppendsLogEntry_ToExistingDeployment()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("ApiTest_AddLog");
            var controller = new DeploymentsController(context);

            var logDto = new CreateDeploymentLogDto
            {
                StepName = "Post-Deploy Smoke Test",
                LogLevel = "Info",
                Message = "HTTP GET /health returned 200 OK within 12ms."
            };

            // Act
            var actionResult = await controller.AddLog(1, logDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var createdLog = Assert.IsType<DeploymentLogDto>(okResult.Value);
            Assert.Equal(1, createdLog.DeploymentRecordId);
            Assert.Equal("Post-Deploy Smoke Test", createdLog.StepName);
        }
    }
}
