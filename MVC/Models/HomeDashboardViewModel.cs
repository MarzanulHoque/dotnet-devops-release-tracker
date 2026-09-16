using System.Collections.Generic;
using DotnetProject.Core.DTOs;
using DotnetProject.Core.Entities;

namespace DotnetProject.Web.Models
{
    public class HomeDashboardViewModel
    {
        public int TotalDeployments { get; set; }
        public int SuccessDeployments { get; set; }
        public int ProductionDeployments { get; set; }
        public int StagingDeployments { get; set; }
        public int SuccessRatePercentage { get; set; } = 100;
        public int ActiveProjectsCount { get; set; }
        public bool IsApiHealthy { get; set; } = true;
        public IReadOnlyList<DeploymentRecord> RecentDeployments { get; set; } = new List<DeploymentRecord>();
        public IReadOnlyList<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    }
}
