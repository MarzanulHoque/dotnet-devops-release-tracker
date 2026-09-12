using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetProject.Core.Entities
{
    /// <summary>
    /// Represents an individual release / deployment event in the system.
    /// </summary>
    public class DeploymentRecord
    {
        [Key]
        public int Id { get; set; }

        public int? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        [Required(ErrorMessage = "Application name is required")]
        [StringLength(100)]
        [Display(Name = "Application Name")]
        public string AppName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Release version is required")]
        [StringLength(50)]
        [Display(Name = "Version")]
        public string Version { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target environment is required")]
        [StringLength(50)]
        [Display(Name = "Environment")]
        public string Environment { get; set; } = "Production"; // Development, Staging, Production

        [Required(ErrorMessage = "Deployment status is required")]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Successful"; // Successful, Failed, In-Progress

        [StringLength(100)]
        [Display(Name = "Deployed By")]
        public string DeployedBy { get; set; } = "GitHub Actions (CI/CD)";

        [StringLength(40)]
        [Display(Name = "Commit Hash")]
        public string CommitHash { get; set; } = "HEAD";

        [Display(Name = "Deployed At")]
        public DateTime DeployedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Duration (Seconds)")]
        public int ExecutionDurationSeconds { get; set; } = 45;

        [StringLength(500)]
        [Display(Name = "Release Notes")]
        public string? Notes { get; set; }

        // Navigation property: One deployment has multiple pipeline execution logs
        public ICollection<DeploymentLog> Logs { get; set; } = new List<DeploymentLog>();
    }
}
