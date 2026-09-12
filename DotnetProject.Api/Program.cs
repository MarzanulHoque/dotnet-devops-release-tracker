using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using DotnetProject.Core.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Add API Controllers & JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();

// 2. Configure Swagger / OpenAPI documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DevOps Release Tracker REST API",
        Version = "v1",
        Description = "Production RESTful API for automated deployment tracking, pipeline telemetry, and CI/CD webhook integrations.",
        Contact = new OpenApiContact
        {
            Name = "Platform Engineering Team",
            Url = new Uri("https://github.com/MarzanulHoque/dotnet-devops-release-tracker")
        }
    });
});

// 3. Configure Database Context (Pomelo MySQL or In-Memory fallback)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbProvider = builder.Configuration["DatabaseProvider"];

if (string.IsNullOrWhiteSpace(connectionString) || string.Equals(dbProvider, "InMemory", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("DevOpsDeploymentsDb"));
}
else
{
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

// 4. Configure CORS for open API consumption
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. Configure Forwarded Headers for Nginx Reverse Proxy on AWS EC2
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// 6. Database schema auto-creation and seed verification
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
            logger.LogInformation("REST API Database migrations applied successfully.");
        }
        else
        {
            context.Database.EnsureCreated();
            logger.LogInformation("In-Memory REST API Database initialized.");
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning("REST API Database initialization warning: {Message}", ex.Message);
    }
}

// 7. Middleware Pipeline
app.UseForwardedHeaders();

// Always expose Swagger in both Development and Production for demonstration & API testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevOps Release Tracker API v1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();

// Expose Program class for WebApplicationFactory in integration tests
public partial class Program { }
