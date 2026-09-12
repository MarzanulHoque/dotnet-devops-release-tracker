using System.ComponentModel.DataAnnotations;

namespace DotnetProject.Core.DTOs
{
    public class UpdateDeploymentDto
    {
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Successful"; // Successful, Failed, In-Progress

        [StringLength(500)]
        public string? Notes { get; set; }

        public int? ExecutionDurationSeconds { get; set; }
    }
}
