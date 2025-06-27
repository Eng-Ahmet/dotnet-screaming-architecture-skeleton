namespace Features.Login.Service;

using System.Threading.Tasks;
using Api.Features.Register.Model;
using Api.Features.Register.Repository;
using Api.Infrastructure.Enums;
using Api.Utils;
using Features.Login.DTO;
using Microsoft.AspNetCore.Mvc;

public class LoginService
{
    private readonly IUserRepository _userRepository;

    public LoginService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> LoginAsync([FromBody] LoginRequest request)
    {
        User user = await _userRepository.GetByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!HashHelper.VerifyPassword(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (user.Status != AccountStatus.Active)
        {
            throw new UnauthorizedAccessException("Account is not active.");
        }

        return user;
    }
}