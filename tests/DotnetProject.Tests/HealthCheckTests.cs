using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetProject.Web.Controllers;
using DotnetProject.Core.Data;
using Xunit;

namespace DotnetProject.Tests
{
    public class HealthCheckTests
    {
        private ApplicationDbContext CreateInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task HealthEndpoint_ReturnsOkOrStatus200_WithDiagnostics()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("HealthCheckDb");
            var controller = new HealthController(context);

            // Act
            var result = await controller.Get();

            // Assert
            Assert.NotNull(result);
            if (result is OkObjectResult okResult)
            {
                Assert.Equal(200, okResult.StatusCode);
                Assert.NotNull(okResult.Value);
            }
            else if (result is ObjectResult objectResult)
            {
                Assert.Equal(200, objectResult.StatusCode);
                Assert.NotNull(objectResult.Value);
            }
            else
            {
                Assert.Fail("Expected OkObjectResult or ObjectResult with status 200");
            }
        }
    }
}
