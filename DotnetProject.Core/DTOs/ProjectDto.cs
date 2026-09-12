using System;
using System.ComponentModel.DataAnnotations;

namespace DotnetProject.Core.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(255)]
        public string? RepositoryUrl { get; set; }

        [StringLength(100)]
        public string Owner { get; set; } = "DevOps Platform Team";

        [StringLength(100)]
        public string TechStack { get; set; } = "ASP.NET Core 8.0, MySQL";

        public DateTime CreatedAt { get; set; }
        public int TotalDeployments { get; set; }
    }

    public class CreateProjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(255)]
        public string? RepositoryUrl { get; set; }

        [StringLength(100)]
        public string Owner { get; set; } = "DevOps Platform Team";

        [StringLength(100)]
        public string TechStack { get; set; } = "ASP.NET Core 8.0, MySQL";
    }
}
