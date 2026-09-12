using System;

namespace DotnetProject.Core.DTOs
{
    public class DeploymentResponseDto
    {
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string AppName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DeployedBy { get; set; } = string.Empty;
        public string CommitHash { get; set; } = string.Empty;
        public DateTime DeployedAt { get; set; }
        public int ExecutionDurationSeconds { get; set; }
        public string? Notes { get; set; }
        public int LogCount { get; set; }
    }
}
