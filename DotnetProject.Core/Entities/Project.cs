using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotnetProject.Core.Entities
{
    /// <summary>
    /// Represents an application/service registered in the DevOps ecosystem.
    /// </summary>
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100)]
        [Display(Name = "Project Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Repository URL")]
        public string? RepositoryUrl { get; set; }

        [StringLength(100)]
        [Display(Name = "Owner / Team")]
        public string Owner { get; set; } = "DevOps Platform Team";

        [StringLength(100)]
        [Display(Name = "Tech Stack")]
        public string TechStack { get; set; } = "ASP.NET Core 8.0, MySQL";

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property: One project has many deployment records
        public ICollection<DeploymentRecord> Deployments { get; set; } = new List<DeploymentRecord>();
    }
}
