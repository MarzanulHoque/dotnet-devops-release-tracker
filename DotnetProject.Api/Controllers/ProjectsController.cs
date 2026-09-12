using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotnetProject.Core.Data;
using DotnetProject.Core.DTOs;
using DotnetProject.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all registered software projects and their deployment count.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
        {
            var projects = await _context.Projects
                .Include(p => p.Deployments)
                .OrderBy(p => p.Name)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    RepositoryUrl = p.RepositoryUrl,
                    Owner = p.Owner,
                    TechStack = p.TechStack,
                    CreatedAt = p.CreatedAt,
                    TotalDeployments = p.Deployments.Count
                })
                .ToListAsync();

            return Ok(projects);
        }

        /// <summary>
        /// Retrieves a project by its unique ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Deployments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound(new { message = $"Project with ID {id} was not found." });
            }

            var response = new
            {
                project.Id,
                project.Name,
                project.Slug,
                project.RepositoryUrl,
                project.Owner,
                project.TechStack,
                project.CreatedAt,
                Deployments = project.Deployments
                    .OrderByDescending(d => d.DeployedAt)
                    .Select(d => new DeploymentResponseDto
                    {
                        Id = d.Id,
                        AppName = d.AppName,
                        Version = d.Version,
                        Environment = d.Environment,
                        Status = d.Status,
                        DeployedBy = d.DeployedBy,
                        CommitHash = d.CommitHash,
                        DeployedAt = d.DeployedAt,
                        ExecutionDurationSeconds = d.ExecutionDurationSeconds,
                        Notes = d.Notes
                    })
            };

            return Ok(response);
        }

        /// <summary>
        /// Registers a new project in the DevOps ecosystem.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var slugExists = await _context.Projects.AnyAsync(p => p.Slug == dto.Slug);
            if (slugExists)
            {
                return Conflict(new { message = $"A project with slug '{dto.Slug}' already exists." });
            }

            var project = new Project
            {
                Name = dto.Name.Trim(),
                Slug = dto.Slug.Trim().ToLower(),
                RepositoryUrl = dto.RepositoryUrl?.Trim(),
                Owner = dto.Owner.Trim(),
                TechStack = dto.TechStack.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var response = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Slug = project.Slug,
                RepositoryUrl = project.RepositoryUrl,
                Owner = project.Owner,
                TechStack = project.TechStack,
                CreatedAt = project.CreatedAt,
                TotalDeployments = 0
            };

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, response);
        }
    }
}
