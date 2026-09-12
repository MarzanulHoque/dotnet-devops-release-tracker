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
    public class DeploymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeploymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all deployment records with optional filtering by environment, status, or search term.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeploymentResponseDto>>> GetAll(
            [FromQuery] string? environment,
            [FromQuery] string? status,
            [FromQuery] int? projectId,
            [FromQuery] string? search)
        {
            var query = _context.Deployments
                .Include(d => d.Project)
                .Include(d => d.Logs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(environment))
            {
                query = query.Where(d => d.Environment == environment);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(d => d.Status == status);
            }

            if (projectId.HasValue)
            {
                query = query.Where(d => d.ProjectId == projectId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(d => d.AppName.ToLower().Contains(term) ||
                                         d.Version.ToLower().Contains(term) ||
                                         d.CommitHash.ToLower().Contains(term));
            }

            var records = await query
                .OrderByDescending(d => d.DeployedAt)
                .Select(d => new DeploymentResponseDto
                {
                    Id = d.Id,
                    ProjectId = d.ProjectId,
                    ProjectName = d.Project != null ? d.Project.Name : null,
                    AppName = d.AppName,
                    Version = d.Version,
                    Environment = d.Environment,
                    Status = d.Status,
                    DeployedBy = d.DeployedBy,
                    CommitHash = d.CommitHash,
                    DeployedAt = d.DeployedAt,
                    ExecutionDurationSeconds = d.ExecutionDurationSeconds,
                    Notes = d.Notes,
                    LogCount = d.Logs.Count
                })
                .ToListAsync();

            return Ok(records);
        }

        /// <summary>
        /// Retrieves a specific deployment record by its ID including its audit logs.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var record = await _context.Deployments
                .Include(d => d.Project)
                .Include(d => d.Logs)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (record == null)
            {
                return NotFound(new { message = $"Deployment with ID {id} was not found." });
            }

            var response = new
            {
                record.Id,
                record.ProjectId,
                ProjectName = record.Project?.Name,
                record.AppName,
                record.Version,
                record.Environment,
                record.Status,
                record.DeployedBy,
                record.CommitHash,
                record.DeployedAt,
                record.ExecutionDurationSeconds,
                record.Notes,
                Logs = record.Logs.OrderBy(l => l.Timestamp).Select(l => new DeploymentLogDto
                {
                    Id = l.Id,
                    DeploymentRecordId = l.DeploymentRecordId,
                    StepName = l.StepName,
                    LogLevel = l.LogLevel,
                    Message = l.Message,
                    Timestamp = l.Timestamp
                })
            };

            return Ok(response);
        }

        /// <summary>
        /// Programmatically creates a new deployment record.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DeploymentResponseDto>> Create([FromBody] CreateDeploymentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = new DeploymentRecord
            {
                ProjectId = dto.ProjectId,
                AppName = dto.AppName.Trim(),
                Version = dto.Version.Trim(),
                Environment = dto.Environment.Trim(),
                Status = dto.Status.Trim(),
                DeployedBy = string.IsNullOrWhiteSpace(dto.DeployedBy) ? "REST API Client" : dto.DeployedBy.Trim(),
                CommitHash = string.IsNullOrWhiteSpace(dto.CommitHash) ? "HEAD" : dto.CommitHash.Trim(),
                ExecutionDurationSeconds = dto.ExecutionDurationSeconds,
                DeployedAt = DateTime.UtcNow,
                Notes = dto.Notes
            };

            _context.Deployments.Add(entity);
            await _context.SaveChangesAsync();

            var response = new DeploymentResponseDto
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                AppName = entity.AppName,
                Version = entity.Version,
                Environment = entity.Environment,
                Status = entity.Status,
                DeployedBy = entity.DeployedBy,
                CommitHash = entity.CommitHash,
                DeployedAt = entity.DeployedAt,
                ExecutionDurationSeconds = entity.ExecutionDurationSeconds,
                Notes = entity.Notes,
                LogCount = 0
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, response);
        }

        /// <summary>
        /// Updates the status or notes of an existing deployment.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDeploymentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var record = await _context.Deployments.FindAsync(id);
            if (record == null)
            {
                return NotFound(new { message = $"Deployment with ID {id} was not found." });
            }

            record.Status = dto.Status.Trim();
            if (dto.Notes != null)
            {
                record.Notes = dto.Notes;
            }
            if (dto.ExecutionDurationSeconds.HasValue)
            {
                record.ExecutionDurationSeconds = dto.ExecutionDurationSeconds.Value;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a deployment record and all associated execution logs.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.Deployments.FindAsync(id);
            if (record == null)
            {
                return NotFound(new { message = $"Deployment with ID {id} was not found." });
            }

            _context.Deployments.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Automated CI/CD Webhook Endpoint: Ingests automated deployment events from GitHub Actions.
        /// </summary>
        [HttpPost("webhook")]
        public async Task<ActionResult<DeploymentResponseDto>> Webhook([FromBody] CiWebhookPayloadDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find or link to project by repository name
            var project = await _context.Projects.FirstOrDefaultAsync(p =>
                p.RepositoryUrl != null && p.RepositoryUrl.Contains(payload.Repository));

            var record = new DeploymentRecord
            {
                ProjectId = project?.Id,
                AppName = string.IsNullOrWhiteSpace(payload.Repository) ? "CI/CD Pipeline" : payload.Repository,
                Version = payload.ReleaseVersion,
                Environment = payload.Environment,
                Status = payload.Status,
                DeployedBy = string.IsNullOrWhiteSpace(payload.Actor) ? "GitHub Actions" : payload.Actor,
                CommitHash = string.IsNullOrWhiteSpace(payload.CommitSha) ? "HEAD" : payload.CommitSha,
                DeployedAt = DateTime.UtcNow,
                ExecutionDurationSeconds = 40,
                Notes = payload.Summary ?? "Automated deployment triggered via CI/CD Webhook."
            };

            _context.Deployments.Add(record);
            await _context.SaveChangesAsync();

            // Automatically add an initial pipeline log entry
            var initialLog = new DeploymentLog
            {
                DeploymentRecordId = record.Id,
                StepName = "CI/CD Webhook Ingestion",
                LogLevel = payload.Status == "Failed" ? "Error" : "Info",
                Message = $"Webhook received from {record.DeployedBy} for commit {record.CommitHash}. Status: {record.Status}.",
                Timestamp = DateTime.UtcNow
            };

            _context.DeploymentLogs.Add(initialLog);
            await _context.SaveChangesAsync();

            var response = new DeploymentResponseDto
            {
                Id = record.Id,
                ProjectId = record.ProjectId,
                ProjectName = project?.Name,
                AppName = record.AppName,
                Version = record.Version,
                Environment = record.Environment,
                Status = record.Status,
                DeployedBy = record.DeployedBy,
                CommitHash = record.CommitHash,
                DeployedAt = record.DeployedAt,
                ExecutionDurationSeconds = record.ExecutionDurationSeconds,
                Notes = record.Notes,
                LogCount = 1
            };

            return CreatedAtAction(nameof(GetById), new { id = record.Id }, response);
        }

        /// <summary>
        /// Appends a new pipeline execution log to a specific deployment record.
        /// </summary>
        [HttpPost("{id:int}/logs")]
        public async Task<ActionResult<DeploymentLogDto>> AddLog(int id, [FromBody] CreateDeploymentLogDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deploymentExists = await _context.Deployments.AnyAsync(d => d.Id == id);
            if (!deploymentExists)
            {
                return NotFound(new { message = $"Deployment with ID {id} was not found." });
            }

            var log = new DeploymentLog
            {
                DeploymentRecordId = id,
                StepName = dto.StepName.Trim(),
                LogLevel = dto.LogLevel.Trim(),
                Message = dto.Message.Trim(),
                Timestamp = DateTime.UtcNow
            };

            _context.DeploymentLogs.Add(log);
            await _context.SaveChangesAsync();

            return Ok(new DeploymentLogDto
            {
                Id = log.Id,
                DeploymentRecordId = log.DeploymentRecordId,
                StepName = log.StepName,
                LogLevel = log.LogLevel,
                Message = log.Message,
                Timestamp = log.Timestamp
            });
        }
    }
}
