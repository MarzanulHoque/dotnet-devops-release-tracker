using System.ComponentModel.DataAnnotations;

namespace DotnetProject.Core.DTOs
{
    /// <summary>
    /// Lightweight payload sent by automated CI/CD pipelines (e.g., GitHub Actions workflow step)
    /// to register an automatic deployment event.
    /// </summary>
    public class CiWebhookPayloadDto
    {
        [Required]
        [StringLength(100)]
        public string Repository { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ReleaseVersion { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Environment { get; set; } = "Production";

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Successful";

        [StringLength(40)]
        public string CommitSha { get; set; } = string.Empty;

        [StringLength(100)]
        public string Actor { get; set; } = "GitHub Actions";

        [StringLength(500)]
        public string? Summary { get; set; }
    }
}
