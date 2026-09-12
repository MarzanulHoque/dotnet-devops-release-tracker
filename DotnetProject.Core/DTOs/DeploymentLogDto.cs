using System;
using System.ComponentModel.DataAnnotations;

namespace DotnetProject.Core.DTOs
{
    public class DeploymentLogDto
    {
        public int Id { get; set; }
        public int DeploymentRecordId { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string LogLevel { get; set; } = "Info";
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class CreateDeploymentLogDto
    {
        [Required]
        [StringLength(100)]
        public string StepName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string LogLevel { get; set; } = "Info";

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
