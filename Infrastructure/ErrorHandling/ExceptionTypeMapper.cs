using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace Api.Infrastructure.ErrorHandling;

public enum ErrorCategory
{
    InvalidInput = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    ValidationError = 422,
    ServerError = 500,
    DatabaseError = 501,
    TokenError = 498,
    Unknown = 520
}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public ErrorCategory Type { get; set; }
    public string Message { get; set; } = string.Empty;
}

public static class ExceptionTypeMapper
{
    public static ErrorDetails Map(Exception ex)
    {
        return ex switch
        {
            // ✅ Invalid Input (400 Bad Request)
            ArgumentNullException => Create(ErrorCategory.InvalidInput, "A required argument was null."),
            ArgumentException => Create(ErrorCategory.InvalidInput, "An argument provided was invalid."),
            FormatException => Create(ErrorCategory.InvalidInput, "Input format is incorrect."),
            JsonException => Create(ErrorCategory.InvalidInput, "Malformed JSON input."),
            InvalidOperationException => Create(ErrorCategory.InvalidInput, "The operation is not allowed in this context."),

            // ✅ Authentication & Authorization (401/403)
            AuthenticationException => Create(ErrorCategory.Unauthorized, "Authentication failed. Token is missing or invalid."),
            SecurityTokenExpiredException => Create(ErrorCategory.TokenError, "The provided token has expired."),
            SecurityTokenInvalidSignatureException => Create(ErrorCategory.TokenError, "The token signature is invalid."),
            SecurityTokenValidationException => Create(ErrorCategory.TokenError, "The token validation failed."),
            SecurityTokenDecryptionFailedException => Create(ErrorCategory.TokenError, "Token decryption failed."),

            // ✅ Not Found or Resource issues (404/409)
            KeyNotFoundException => Create(ErrorCategory.NotFound, "The requested resource was not found."),
            InvalidCastException => Create(ErrorCategory.Conflict, "A type conversion error occurred."),

            // ✅ Validation error (422)
            ApplicationException => Create(ErrorCategory.ValidationError, "A validation error occurred. Please check your input."),

            // ✅ Database errors (501)
            DbUpdateException => Create(ErrorCategory.DatabaseError, "An error occurred while updating the database."),
            DbException => Create(ErrorCategory.DatabaseError, "A general database error occurred."),

            UnauthorizedAccessException => Create(ErrorCategory.Forbidden, "You do not have permission to perform this action."),

            // ✅ Catch-all (500)
            _ => Create(ErrorCategory.ServerError, "An unexpected server error occurred.")
        };
    }

    private static ErrorDetails Create(ErrorCategory type, string message)
    {
        return new ErrorDetails
        {
            StatusCode = (int)type,
            Type = type,
            Message = message
        };
    }
}
