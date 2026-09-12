using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetProject.Web.Controllers;
using Xunit;

namespace DotnetProject.Tests
{
    public class HealthCheckTests
    {
        [Fact]
        public async Task HealthEndpoint_ReturnsOkOrStatus200_WithApiTierDiagnostics()
        {
            // Arrange
            var fakeApiClient = new FakeDeploymentApiClient();
            var controller = new HealthController(fakeApiClient);

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
