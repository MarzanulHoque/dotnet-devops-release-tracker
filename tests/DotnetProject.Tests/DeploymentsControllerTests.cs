using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetProject.Web.Controllers;
using DotnetProject.Web.Services;
using DotnetProject.Core.DTOs;
using DotnetProject.Core.Entities;
using Xunit;

namespace DotnetProject.Tests
{
    public class FakeDeploymentApiClient : IDeploymentApiClient
    {
        public List<DeploymentRecord> Deployments { get; } = new()
        {
            new DeploymentRecord
            {
                Id = 1,
                AppName = "ReleaseTracker",
                Version = "v1.0.0",
                Environment = "Production",
                Status = "Successful",
                DeployedBy = "Admin",
                CommitHash = "abc1234",
                DeployedAt = DateTime.UtcNow.AddHours(-2)
            },
            new DeploymentRecord
            {
                Id = 2,
                AppName = "PaymentsService",
                Version = "v1.1.0",
                Environment = "Staging",
                Status = "Successful",
                DeployedBy = "CI Bot",
                CommitHash = "def5678",
                DeployedAt = DateTime.UtcNow.AddHours(-1)
            }
        };

        public Task<IReadOnlyList<DeploymentRecord>> GetDeploymentsAsync(string? envFilter = null, string? statusFilter = null, string? search = null)
        {
            var query = Deployments.AsQueryable();
            if (!string.IsNullOrEmpty(envFilter))
            {
                query = query.Where(d => d.Environment == envFilter);
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(d => d.Status == statusFilter);
            }
            return Task.FromResult<IReadOnlyList<DeploymentRecord>>(query.ToList());
        }

        public Task<DeploymentRecord?> GetDeploymentByIdAsync(int id)
        {
            var record = Deployments.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(record);
        }

        public Task<bool> CreateDeploymentAsync(CreateDeploymentDto dto)
        {
            var newId = Deployments.Count > 0 ? Deployments.Max(d => d.Id) + 1 : 1;
            Deployments.Add(new DeploymentRecord
            {
                Id = newId,
                AppName = dto.AppName,
                Version = dto.Version,
                Environment = dto.Environment,
                Status = dto.Status,
                DeployedBy = dto.DeployedBy,
                CommitHash = dto.CommitHash,
                DeployedAt = DateTime.UtcNow,
                Notes = dto.Notes
            });
            return Task.FromResult(true);
        }

        public Task<bool> UpdateDeploymentAsync(int id, UpdateDeploymentDto dto)
        {
            var record = Deployments.FirstOrDefault(d => d.Id == id);
            if (record == null) return Task.FromResult(false);
            record.Status = dto.Status;
            record.Notes = dto.Notes;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteDeploymentAsync(int id)
        {
            var record = Deployments.FirstOrDefault(d => d.Id == id);
            if (record == null) return Task.FromResult(false);
            Deployments.Remove(record);
            return Task.FromResult(true);
        }

        public Task<IReadOnlyList<ProjectDto>> GetProjectsAsync()
        {
            var list = new List<ProjectDto>
            {
                new ProjectDto { Id = 1, Name = "ReleaseTracker", Slug = "release-tracker" }
            };
            return Task.FromResult<IReadOnlyList<ProjectDto>>(list);
        }

        public Task<bool> HealthCheckAsync() => Task.FromResult(true);
    }

    public class DeploymentsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithAllRecords_FromApiClient()
        {
            // Arrange
            var apiClient = new FakeDeploymentApiClient();
            var controller = new DeploymentsController(apiClient);

            // Act
            var result = await controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<DeploymentRecord>>(viewResult.Model);
            Assert.NotEmpty(model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_FiltersByEnvironmentCorrectly_ThroughApiClient()
        {
            // Arrange
            var apiClient = new FakeDeploymentApiClient();
            var controller = new DeploymentsController(apiClient);

            // Act
            var result = await controller.Index("Production", null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<DeploymentRecord>>(viewResult.Model);
            Assert.Single(model);
            Assert.All(model, item => Assert.Equal("Production", item.Environment));
        }

        [Fact]
        public async Task Create_ValidRecord_DelegatesToApiClientAndRedirects()
        {
            // Arrange
            var apiClient = new FakeDeploymentApiClient();
            var controller = new DeploymentsController(apiClient);
            var newRecord = new DeploymentRecord
            {
                AppName = "BillingApi",
                Version = "v3.0.0",
                Environment = "Production",
                Status = "Successful",
                DeployedBy = "GitHub Actions CI/CD",
                CommitHash = "f9e8d7c",
                Notes = "Automated deployment test"
            };

            // Act
            var result = await controller.Create(newRecord);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            var saved = apiClient.Deployments.FirstOrDefault(d => d.AppName == "BillingApi");
            Assert.NotNull(saved);
            Assert.Equal("v3.0.0", saved.Version);
        }

        [Fact]
        public async Task Details_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var apiClient = new FakeDeploymentApiClient();
            var controller = new DeploymentsController(apiClient);

            // Act
            var result = await controller.Details(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
