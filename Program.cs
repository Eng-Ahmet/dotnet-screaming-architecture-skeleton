using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

Startup startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, builder.Environment);

app.Run();
