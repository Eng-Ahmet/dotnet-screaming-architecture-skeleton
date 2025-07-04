using Api.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var url = builder.Configuration["ApplicationUrl"] ?? "http://localhost:5000";
builder.WebHost.UseUrls(url);

// 💡 Configure Serilog with colors and nice template
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] 🚀 {Message:lj}{NewLine}{Exception}",
        theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate)
    .WriteTo.File("Logs/log.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog(); // Use Serilog instead of default logger

// 🔧 Start message
Log.Information("🟢 Starting up the API...");

Startup startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// 🔧 Build the application
var app = builder.Build();

// ✅ Welcome endpoint using Minimal API
app.MapGet("/", () =>
{
    return Results.Ok(new { Message = "Welcome to the API!" });
});

// 🔧 Configure the application
startup.Configure(app, builder.Environment);

// 🔧 Ready message

// ✅ Extract applicationUrl from the launchSettings.json

Log.Information("✅ Application is running at: {Url}", url);
Log.Information("📡 Environment: {Env}", builder.Environment.EnvironmentName);


app.Run();

// 🔴 On exit
Log.CloseAndFlush();
