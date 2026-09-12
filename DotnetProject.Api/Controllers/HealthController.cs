using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using DotnetProject.Core.Data;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotnetProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private static readonly DateTime _startTime = DateTime.UtcNow;

        public HealthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Diagnostic health check endpoint returning API uptime, environment, and MySQL database connectivity.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            var canConnect = false;
            string dbMessage;
            var provider = _context.Database.ProviderName ?? "Unknown";

            try
            {
                canConnect = await _context.Database.CanConnectAsync();
                dbMessage = canConnect ? "Connected" : "Connection returned false";
            }
            catch (Exception ex)
            {
                dbMessage = $"Connection failed: {ex.Message}";
            }

            var uptime = DateTime.UtcNow - _startTime;
            var isHealthy = canConnect;

            var response = new
            {
                status = isHealthy ? "Healthy" : "Degraded",
                timestamp = DateTime.UtcNow,
                service = "DotnetProject.Api",
                version = "2.0.0 (Phase 2 Decoupled REST API)",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                uptime = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                hostname = Environment.MachineName,
                database = new
                {
                    provider,
                    connected = canConnect,
                    message = dbMessage
                }
            };

            return isHealthy ? Ok(response) : StatusCode(503, response);
        }
    }
}
