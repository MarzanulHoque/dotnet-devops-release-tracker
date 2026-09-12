using Microsoft.AspNetCore.Mvc;
using DotnetProject.Web.Services;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DotnetProject.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IDeploymentApiClient _apiClient;
        private static readonly DateTime StartTime = DateTime.UtcNow;

        public HealthController(IDeploymentApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var apiHealthy = await _apiClient.HealthCheckAsync();
            var uptime = DateTime.UtcNow - StartTime;
            var assembly = Assembly.GetExecutingAssembly().GetName();

            var healthReport = new
            {
                status = apiHealthy ? "Healthy" : "Degraded",
                timestamp = DateTime.UtcNow.ToString("o"),
                service = assembly.Name ?? "DotnetProject.Web",
                version = assembly.Version?.ToString() ?? "2.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                architecture = "3-Tier (MVC ──► REST API ──► MySQL)",
                uptime = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                hostname = Environment.MachineName,
                apiTier = new
                {
                    connected = apiHealthy,
                    status = apiHealthy ? "Online" : "Unreachable"
                }
            };

            return apiHealthy ? Ok(healthReport) : StatusCode(200, healthReport);
        }
    }
}
