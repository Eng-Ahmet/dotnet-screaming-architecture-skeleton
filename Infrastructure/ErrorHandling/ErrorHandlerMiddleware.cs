using System.Text.Json;
using Serilog;

namespace Api.Infrastructure.ErrorHandling;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("An error occurred: {ErrorMessage}", ex.Message);
            _logger.LogInformation("Work started at {time}", DateTime.UtcNow);
            _logger.LogWarning("This is a test warning");
            _logger.LogError("Something went wrong!");

            var error = ErrorResponse.FromException(context, ex);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.StatusCode;

            var result = JsonSerializer.Serialize(error);
            await context.Response.WriteAsync(result);
        }
    }

}
