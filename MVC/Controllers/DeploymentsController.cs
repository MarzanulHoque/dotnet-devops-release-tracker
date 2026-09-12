using Microsoft.AspNetCore.Mvc;
using DotnetProject.Core.DTOs;
using DotnetProject.Core.Entities;
using DotnetProject.Web.Services;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetProject.Web.Controllers
{
    /// <summary>
    /// Presentation Controller for Release Management.
    /// Operates as a pure API consumer over HTTP (True 3-Tier Architecture).
    /// </summary>
    public class DeploymentsController : Controller
    {
        private readonly IDeploymentApiClient _apiClient;

        public DeploymentsController(IDeploymentApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Deployments
        public async Task<IActionResult> Index(string? envFilter, string? statusFilter)
        {
            var list = await _apiClient.GetDeploymentsAsync(envFilter, statusFilter);

            ViewBag.CurrentEnv = envFilter;
            ViewBag.CurrentStatus = statusFilter;
            ViewBag.TotalCount = list.Count;
            ViewBag.SuccessCount = list.Count(d => d.Status == "Successful");
            ViewBag.ProdCount = list.Count(d => d.Environment == "Production");

            return View(list);
        }

        // GET: Deployments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _apiClient.GetDeploymentByIdAsync(id.Value);
            if (record == null) return NotFound();

            return View(record);
        }

        // GET: Deployments/Create
        public IActionResult Create()
        {
            var model = new DeploymentRecord
            {
                DeployedAt = System.DateTime.UtcNow,
                Status = "Successful",
                Environment = "Production",
                DeployedBy = "GitHub Actions CI/CD"
            };
            return View(model);
        }

        // POST: Deployments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppName,Version,Environment,Status,DeployedBy,CommitHash,DeployedAt,Notes")] DeploymentRecord record)
        {
            if (ModelState.IsValid)
            {
                var dto = new CreateDeploymentDto
                {
                    ProjectId = record.ProjectId,
                    AppName = record.AppName,
                    Version = record.Version,
                    Environment = record.Environment,
                    Status = record.Status,
                    DeployedBy = record.DeployedBy,
                    CommitHash = string.IsNullOrWhiteSpace(record.CommitHash) ? "HEAD" : record.CommitHash,
                    ExecutionDurationSeconds = record.ExecutionDurationSeconds,
                    Notes = record.Notes
                };

                var success = await _apiClient.CreateDeploymentAsync(dto);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to save deployment to REST API tier. Verify API service is reachable.");
            }
            return View(record);
        }

        // GET: Deployments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _apiClient.GetDeploymentByIdAsync(id.Value);
            if (record == null) return NotFound();

            return View(record);
        }

        // POST: Deployments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppName,Version,Environment,Status,DeployedBy,CommitHash,DeployedAt,Notes")] DeploymentRecord record)
        {
            if (id != record.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var dto = new UpdateDeploymentDto
                {
                    AppName = record.AppName,
                    Version = record.Version,
                    Environment = record.Environment,
                    Status = record.Status,
                    DeployedBy = record.DeployedBy,
                    CommitHash = record.CommitHash,
                    ExecutionDurationSeconds = record.ExecutionDurationSeconds,
                    Notes = record.Notes
                };

                var success = await _apiClient.UpdateDeploymentAsync(id, dto);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to update deployment via REST API tier.");
            }
            return View(record);
        }

        // GET: Deployments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _apiClient.GetDeploymentByIdAsync(id.Value);
            if (record == null) return NotFound();

            return View(record);
        }

        // POST: Deployments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiClient.DeleteDeploymentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
