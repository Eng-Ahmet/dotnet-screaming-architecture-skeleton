namespace Api.Features.Register.Controller;

using Api.Features.Register.DTO;
using Api.Features.Register.Service;
using Api.Infrastructure.ErrorHandling;
using Api.Infrastructure.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly RegisterService _registerService;

    public RegisterController(RegisterService registerService)
    {
        _registerService = registerService;
    }

    [HttpGet]
    public IActionResult Get()
    {

        var response = SuccessResponse<object>.FromResult(HttpContext, null, "Welcome to the registration endpoint");
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _registerService.RegisterAsync(request);
            var response = SuccessResponse<object>.FromResult(HttpContext, null, "User registered successfully", 201);

            return Ok(response);
        }
        catch (Exception ex)
        {
            var errorResponse = ErrorResponse.FromException(HttpContext, ex);
            // استخدم رمز الحالة المناسب الموجود في errorResponse.StatusCode
            return StatusCode(errorResponse.StatusCode, errorResponse);
        }
    }
}
