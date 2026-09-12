using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetProject.Core.DTOs;
using DotnetProject.Core.Entities;

namespace DotnetProject.Web.Services
{
    /// <summary>
    /// Contract for consuming the decoupled REST Web API tier over HTTP.
    /// Eliminates direct database coupling from the presentation layer.
    /// </summary>
    public interface IDeploymentApiClient
    {
        Task<IReadOnlyList<DeploymentRecord>> GetDeploymentsAsync(string? envFilter = null, string? statusFilter = null, string? search = null);
        Task<DeploymentRecord?> GetDeploymentByIdAsync(int id);
        Task<bool> CreateDeploymentAsync(CreateDeploymentDto dto);
        Task<bool> UpdateDeploymentAsync(int id, UpdateDeploymentDto dto);
        Task<bool> DeleteDeploymentAsync(int id);
        Task<IReadOnlyList<ProjectDto>> GetProjectsAsync();
        Task<bool> HealthCheckAsync();
    }
}
