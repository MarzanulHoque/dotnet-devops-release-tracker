using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetProject.Core.Data;
using DotnetProject.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetProject.Web.Controllers
{
    public class DeploymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeploymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Deployments
        public async Task<IActionResult> Index(string? envFilter, string? statusFilter)
        {
            var query = _context.Deployments.AsQueryable();

            if (!string.IsNullOrEmpty(envFilter))
            {
                query = query.Where(d => d.Environment == envFilter);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(d => d.Status == statusFilter);
            }

            var list = await query.OrderByDescending(d => d.DeployedAt).ToListAsync();

            ViewBag.CurrentEnv = envFilter;
            ViewBag.CurrentStatus = statusFilter;
            ViewBag.TotalCount = await _context.Deployments.CountAsync();
            ViewBag.SuccessCount = await _context.Deployments.CountAsync(d => d.Status == "Successful");
            ViewBag.ProdCount = await _context.Deployments.CountAsync(d => d.Environment == "Production");

            return View(list);
        }

        // GET: Deployments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.Deployments.FirstOrDefaultAsync(m => m.Id == id);
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
                _context.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(record);
        }

        // GET: Deployments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.Deployments.FindAsync(id);
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
                try
                {
                    _context.Update(record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeploymentExists(record.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(record);
        }

        // GET: Deployments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _context.Deployments.FirstOrDefaultAsync(m => m.Id == id);
            if (record == null) return NotFound();

            return View(record);
        }

        // POST: Deployments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.Deployments.FindAsync(id);
            if (record != null)
            {
                _context.Deployments.Remove(record);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DeploymentExists(int id)
        {
            return _context.Deployments.Any(e => e.Id == id);
        }
    }
}
