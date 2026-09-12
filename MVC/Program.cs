using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotnetProject.Core.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC Controllers and Views
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();

// 2. Configure Database Context (MySQL with Pomelo EF Core, injected via AWS / Environment Variables)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbProvider = builder.Configuration["DatabaseProvider"];

if (string.IsNullOrWhiteSpace(connectionString) || string.Equals(dbProvider, "InMemory", StringComparison.OrdinalIgnoreCase))
{
    // If no connection string is provided via AWS or environment variables, fallback gracefully to in-memory DB
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DevOpsDeploymentsDb"));
}
else
{
    // MySQL configuration with retry logic for resilient AWS cloud deployment
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
        options.UseMySql(connectionString, serverVersion, mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
    });
}

// 3. Configure Forwarded Headers for Nginx Reverse Proxy (AWS EC2 deployment)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// 4. Database auto-initialization / schema check
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        logger.LogInformation("Database verified and schema initialized.");
    }
    catch (Exception ex)
    {
        logger.LogWarning("Database connection failed during startup (Ensure MySQL is running): {Message}", ex.Message);
    }
}

// 5. Middleware Pipeline
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();

// Expose Program class for WebApplicationFactory testing in xUnit
public partial class Program { }
