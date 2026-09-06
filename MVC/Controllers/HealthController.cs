using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetProject.Web.Data;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace DotnetProject.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static readonly DateTime StartTime = DateTime.UtcNow;

        public HealthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var canConnectToDb = false;
            string dbStatusMessage;

            try
            {
                canConnectToDb = await _context.Database.CanConnectAsync();
                dbStatusMessage = canConnectToDb ? "Connected" : "Unreachable";
            }
            catch (Exception ex)
            {
                dbStatusMessage = $"Connection failed: {ex.Message}";
            }

            var uptime = DateTime.UtcNow - StartTime;
            var assembly = Assembly.GetExecutingAssembly().GetName();

            var healthReport = new
            {
                status = canConnectToDb ? "Healthy" : "Degraded",
                timestamp = DateTime.UtcNow.ToString("o"),
                service = assembly.Name ?? "DotnetProject.Web",
                version = assembly.Version?.ToString() ?? "1.0.0",
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                uptime = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s",
                hostname = Environment.MachineName,
                database = new
                {
                    provider = _context.Database.ProviderName,
                    connected = canConnectToDb,
                    message = dbStatusMessage
                }
            };

            return canConnectToDb ? Ok(healthReport) : StatusCode(200, healthReport);
        }
    }
}
