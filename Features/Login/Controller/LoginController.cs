namespace Features.Login.Controller;

using System.Threading.Tasks;
using Api.Features.Register.Model;
using Api.Infrastructure.ErrorHandling;
using Api.Infrastructure.Responses;
using Features.Login.DTO;
using Features.Login.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginService _loginService;
    private readonly ILogger<LoginController> _logger;
    private readonly JwtHelper _jwtHelper;

    public LoginController(LoginService loginService, ILogger<LoginController> logger, JwtHelper jwtHelper)
    {
        _logger = logger;
        _jwtHelper = jwtHelper;
        _loginService = loginService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            User user = await _loginService.LoginAsync(request, ipAddress);

            var response = SuccessResponse<object>.FromResult(HttpContext, new
            {
                Token = _jwtHelper.GenerateToken(user.Id, user.Email, [user.UserType.ToString()]),
                Message = "Login successful"
            },
            "Login successful", 200);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ErrorResponse.FromException(HttpContext, ex));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ErrorResponse.FromException(HttpContext, ex));
        }
    }

}