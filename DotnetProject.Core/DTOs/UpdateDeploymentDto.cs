using System.ComponentModel.DataAnnotations;

namespace DotnetProject.Core.DTOs
{
    public class UpdateDeploymentDto
    {
        [StringLength(100)]
        public string? AppName { get; set; }

        [StringLength(50)]
        public string? Version { get; set; }

        [StringLength(50)]
        public string? Environment { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Successful"; // Successful, Failed, In-Progress

        [StringLength(100)]
        public string? DeployedBy { get; set; }

        [StringLength(40)]
        public string? CommitHash { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? ExecutionDurationSeconds { get; set; }
    }
}
