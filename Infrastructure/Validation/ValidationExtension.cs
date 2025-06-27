using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Api.Infrastructure.ErrorHandling;

namespace Api.Infrastructure.Extensions;

public static class ValidationExtension
{
    public static IServiceCollection AddValidationResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => $"{e.Key.Split('.').Last()}: {e.Value?.Errors.First().ErrorMessage}")
                    .ToList();

                var errorResponse = new ErrorResponse
                {
                    Message = "Validation failed. Please check your input.",
                    StatusCode = (int)ErrorCategory.ValidationError,
                    Errors = errors,
                    TraceId = context.HttpContext.TraceIdentifier,
                    Path = context.HttpContext.Request.Path,
                    Method = context.HttpContext.Request.Method,
                    Timestamp = DateTime.UtcNow.ToString("o")
                };

                return new UnprocessableEntityObjectResult(errorResponse); // HTTP 422
            };
        });

        return services;
    }
}
