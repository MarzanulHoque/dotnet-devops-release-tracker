using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DotnetProject.Core.DTOs;
using DotnetProject.Core.Entities;
using Microsoft.Extensions.Logging;

namespace DotnetProject.Web.Services
{
    public class DeploymentApiClient : IDeploymentApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DeploymentApiClient> _logger;

        public DeploymentApiClient(HttpClient httpClient, ILogger<DeploymentApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IReadOnlyList<DeploymentRecord>> GetDeploymentsAsync(string? envFilter = null, string? statusFilter = null, string? search = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrWhiteSpace(envFilter)) queryParams.Add($"environment={Uri.EscapeDataString(envFilter)}");
                if (!string.IsNullOrWhiteSpace(statusFilter)) queryParams.Add($"status={Uri.EscapeDataString(statusFilter)}");
                if (!string.IsNullOrWhiteSpace(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");

                var url = "api/deployments";
                if (queryParams.Count > 0)
                {
                    url += "?" + string.Join("&", queryParams);
                }

                var dtos = await _httpClient.GetFromJsonAsync<List<DeploymentResponseDto>>(url);
                if (dtos == null) return Array.Empty<DeploymentRecord>();

                return dtos.Select(d => new DeploymentRecord
                {
                    Id = d.Id,
                    ProjectId = d.ProjectId,
                    Project = d.ProjectName != null ? new Project { Id = d.ProjectId ?? 0, Name = d.ProjectName } : null,
                    AppName = d.AppName,
                    Version = d.Version,
                    Environment = d.Environment,
                    Status = d.Status,
                    DeployedBy = d.DeployedBy,
                    CommitHash = d.CommitHash,
                    DeployedAt = d.DeployedAt,
                    ExecutionDurationSeconds = d.ExecutionDurationSeconds,
                    Notes = d.Notes
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve deployments from REST API tier.");
                return Array.Empty<DeploymentRecord>();
            }
        }

        public async Task<DeploymentRecord?> GetDeploymentByIdAsync(int id)
        {
            try
            {
                var dto = await _httpClient.GetFromJsonAsync<DeploymentResponseDto>($"api/deployments/{id}");
                if (dto == null) return null;

                return new DeploymentRecord
                {
                    Id = dto.Id,
                    ProjectId = dto.ProjectId,
                    Project = dto.ProjectName != null ? new Project { Id = dto.ProjectId ?? 0, Name = dto.ProjectName } : null,
                    AppName = dto.AppName,
                    Version = dto.Version,
                    Environment = dto.Environment,
                    Status = dto.Status,
                    DeployedBy = dto.DeployedBy,
                    CommitHash = dto.CommitHash,
                    DeployedAt = dto.DeployedAt,
                    ExecutionDurationSeconds = dto.ExecutionDurationSeconds,
                    Notes = dto.Notes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve deployment {Id} from REST API tier.", id);
                return null;
            }
        }

        public async Task<bool> CreateDeploymentAsync(CreateDeploymentDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/deployments", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create deployment via REST API tier.");
                return false;
            }
        }

        public async Task<bool> UpdateDeploymentAsync(int id, UpdateDeploymentDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/deployments/{id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update deployment {Id} via REST API tier.", id);
                return false;
            }
        }

        public async Task<bool> DeleteDeploymentAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/deployments/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete deployment {Id} via REST API tier.", id);
                return false;
            }
        }

        public async Task<IReadOnlyList<ProjectDto>> GetProjectsAsync()
        {
            try
            {
                var dtos = await _httpClient.GetFromJsonAsync<List<ProjectDto>>("api/projects");
                return dtos ?? new List<ProjectDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve projects from REST API tier.");
                return Array.Empty<ProjectDto>();
            }
        }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
