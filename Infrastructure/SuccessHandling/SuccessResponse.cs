using Microsoft.AspNetCore.Http;

namespace Api.Infrastructure.Responses;

public class SuccessResponse<T>
{
    public bool Success => true;
    public string Message { get; set; } = "Request completed successfully.";
    public int StatusCode { get; set; } = 200;
    public T? Data { get; set; }

    // Additional Metadata
    public string? TraceId { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public string? Timestamp { get; set; }

    public static SuccessResponse<T> FromResult(HttpContext context, T? data, string? message = null, int statusCode = 200)
    {
        return new SuccessResponse<T>
        {
            Data = data,
            Message = message ?? "Request completed successfully.",
            StatusCode = statusCode,
            TraceId = context.TraceIdentifier,
            Path = context.Request.Path,
            Method = context.Request.Method,
            Timestamp = DateTime.UtcNow.ToString("o") // ISO 8601 format
        };
    }
}
