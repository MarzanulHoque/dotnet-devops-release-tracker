using System.ComponentModel.DataAnnotations;

namespace DotnetProject.Core.DTOs
{
    public class CreateDeploymentDto
    {
        public int? ProjectId { get; set; }

        [Required(ErrorMessage = "Application name is required")]
        [StringLength(100)]
        public string AppName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Version is required")]
        [StringLength(50)]
        public string Version { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target environment is required")]
        [StringLength(50)]
        public string Environment { get; set; } = "Production";

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        public string Status { get; set; } = "Successful";

        [StringLength(100)]
        public string DeployedBy { get; set; } = "GitHub Actions CI/CD";

        [StringLength(40)]
        public string CommitHash { get; set; } = "HEAD";

        public int ExecutionDurationSeconds { get; set; } = 30;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
