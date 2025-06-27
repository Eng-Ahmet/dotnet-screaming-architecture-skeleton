namespace Api.Infrastructure.ErrorHandling;

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "An error occurred while processing your request.";
    public int StatusCode { get; set; } = 500;
    public List<string>? Errors { get; set; }

    // معلومات إضافية
    public string? TraceId { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public string? Timestamp { get; set; }

    public static ErrorResponse FromException(HttpContext context, Exception ex)
    {
        var errorDetails = ExceptionTypeMapper.Map(ex);

        return new ErrorResponse
        {
            Message = errorDetails.Message,
            StatusCode = errorDetails.StatusCode,
            Errors = new List<string> { ex.Message },
            TraceId = context.TraceIdentifier,
            Path = context.Request.Path,
            Method = context.Request.Method,
            Timestamp = DateTime.UtcNow.ToString("o")
        };
    }

}
