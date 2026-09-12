using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace DotnetProject.Core.Data
{
    /// <summary>
    /// Design-time factory enabling 'dotnet ef migrations' to generate MySQL migrations
    /// independently of runtime configuration or environment variable availability.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

            // Design-time placeholder connection string for schema scaffolding
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3306;Database=devops_deployments;User=root;Password=;",
                serverVersion,
                mySqlOptions =>
                {
                    mySqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
