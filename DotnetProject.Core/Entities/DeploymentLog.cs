using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetProject.Core.Entities
{
    /// <summary>
    /// Represents granular execution logs for each pipeline step of a deployment.
    /// </summary>
    public class DeploymentLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DeploymentRecordId { get; set; }

        [ForeignKey("DeploymentRecordId")]
        public DeploymentRecord? DeploymentRecord { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Pipeline Step")]
        public string StepName { get; set; } = string.Empty; // e.g. "Build", "Unit Tests", "SCP Transfer", "systemd restart"

        [Required]
        [StringLength(20)]
        [Display(Name = "Log Level")]
        public string LogLevel { get; set; } = "Info"; // Info, Warning, Error

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
