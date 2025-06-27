// File: Infrastructure/Authentication/JwtAuthenticationExtension.cs

using Api.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace Api.Infrastructure.Authentication
{
    public static class JwtAuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var secret = config["Jwt:Secret"];
            var issuer = config["Jwt:Issuer"];
            var audience = config["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT secret is not configured.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var errorResponse = new ErrorResponse
                        {
                            Message = "Authentication failed. Token is invalid or expired.",
                            StatusCode = (int)ErrorCategory.Unauthorized,
                            Errors = new List<string> { context.Exception?.Message ?? "Invalid token." },
                            TraceId = context.HttpContext.TraceIdentifier,
                            Path = context.HttpContext.Request.Path,
                            Method = context.HttpContext.Request.Method,
                            Timestamp = DateTime.UtcNow.ToString("o")
                        };

                        var result = JsonSerializer.Serialize(errorResponse);
                        return context.Response.WriteAsync(result);
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var errorResponse = new ErrorResponse
                        {
                            Message = "You are not authorized. Token is missing or invalid.",
                            StatusCode = (int)ErrorCategory.Unauthorized,
                            Errors = new List<string> { context.ErrorDescription ?? "Unauthorized request." },
                            TraceId = context.HttpContext.TraceIdentifier,
                            Path = context.HttpContext.Request.Path,
                            Method = context.HttpContext.Request.Method,
                            Timestamp = DateTime.UtcNow.ToString("o")
                        };

                        var result = JsonSerializer.Serialize(errorResponse);
                        return context.Response.WriteAsync(result);
                    }
                };
            });

            return services;
        }
    }
}
