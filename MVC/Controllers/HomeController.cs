using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotnetProject.Web.Models;

namespace DotnetProject.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DotnetProject.Web.Services.IDeploymentApiClient _apiClient;

    public HomeController(ILogger<HomeController> logger, DotnetProject.Web.Services.IDeploymentApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeDashboardViewModel();
        try
        {
            var deploymentsTask = _apiClient.GetDeploymentsAsync();
            var projectsTask = _apiClient.GetProjectsAsync();
            var healthTask = _apiClient.HealthCheckAsync();

            await Task.WhenAll(deploymentsTask, projectsTask, healthTask);

            var deployments = await deploymentsTask;
            var projects = await projectsTask;
            var isHealthy = await healthTask;

            model.TotalDeployments = deployments.Count;
            model.SuccessDeployments = deployments.Count(d => d.Status == "Successful");
            model.ProductionDeployments = deployments.Count(d => d.Environment == "Production");
            model.StagingDeployments = deployments.Count(d => d.Environment == "Staging");
            model.SuccessRatePercentage = deployments.Count > 0 
                ? (int)Math.Round((double)model.SuccessDeployments / deployments.Count * 100) 
                : 100;
            model.ActiveProjectsCount = projects.Count;
            model.IsApiHealthy = isHealthy;
            model.RecentDeployments = deployments.Take(6).ToList();
            model.Projects = projects;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load telemetry stats for Home Dashboard. Falling back to defaults.");
        }

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
