using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetProject.Web.Controllers;
using DotnetProject.Web.Data;
using DotnetProject.Web.Models;
using Xunit;

namespace DotnetProject.Tests
{
    public class DeploymentsControllerTests
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
        public async Task Index_ReturnsViewWithAllRecords()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("TestDb_IndexAll");
            var controller = new DeploymentsController(context);

            // Act
            var result = await controller.Index(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<DeploymentRecord>>(viewResult.Model);
            Assert.NotEmpty(model);
        }

        [Fact]
        public async Task Index_FiltersByEnvironmentCorrectly()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("TestDb_FilterEnv");
            var controller = new DeploymentsController(context);

            // Act
            var result = await controller.Index("Production", null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<DeploymentRecord>>(viewResult.Model);
            Assert.All(model, item => Assert.Equal("Production", item.Environment));
        }

        [Fact]
        public async Task Create_ValidRecord_RedirectsToIndexAndSaves()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("TestDb_CreateRecord");
            var controller = new DeploymentsController(context);
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

            var saved = await context.Deployments.FirstOrDefaultAsync(d => d.AppName == "BillingApi");
            Assert.NotNull(saved);
            Assert.Equal("v3.0.0", saved.Version);
        }

        [Fact]
        public async Task Details_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("TestDb_DetailsNotFound");
            var controller = new DeploymentsController(context);

            // Act
            var result = await controller.Details(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
