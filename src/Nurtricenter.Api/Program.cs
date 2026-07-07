using Nurtricenter.Api;
using Nurtricenter.Application;
using Nurtricenter.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Logging ───────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// ── Layer registrations ───────────────────────────────────────────
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices();

// ── Run ───────────────────────────────────────────────────────────
var app = builder.Build();

app.UseApiMiddleware();

try
{
    Log.Information("Starting Nurtricenter API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
