
using Api.Features.Register.Repository;
using Api.Features.Register.Service;
using Api.Infrastructure;
using Api.Infrastructure.Authentication;
using Api.Infrastructure.ErrorHandling;
using Api.Infrastructure.Extensions;
using Features.Login.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class Startup
{
    private readonly IConfiguration _config;
    public Startup(IConfiguration config)
    {
        _config = config;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        // Configure MySQL database connection
        var connectionString = _config.GetConnectionString("MySQL");
        services.AddDbContextPool<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // Cross-Origin Resource Sharing (CORS) configuration
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", builder =>
            {
                builder.WithOrigins("*")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        // Configure JSON serialization options
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                options.JsonSerializerOptions.MaxDepth = 64;
            });

        // Configure API behavior options for model validation
        services.AddValidationResponse();

        // Configure jwt authentication
        services.AddJwtAuthentication(_config);

        // Register services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<RegisterService>();
        services.AddScoped<LoginService>();

        services.AddScoped<JwtHelper>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseMiddleware<ErrorHandlerMiddleware>();

        app.UseRouting();

        app.UseCors("AllowSpecificOrigins");

        app.UseAuthentication();   // يجب أن يكون قبل UseAuthorization
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
