using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotnetProject.Web.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC Controllers and Views
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();

// 2. Configure Typed HTTP Client for the REST API Tier (3-Tier Architecture)
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://127.0.0.1:5050";
builder.Services.AddHttpClient<IDeploymentApiClient, DeploymentApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// 3. Configure Forwarded Headers for Nginx Reverse Proxy (AWS EC2 deployment)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// 4. Middleware Pipeline
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
