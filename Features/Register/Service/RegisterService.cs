
using Api.Features.Register.DTO;
using Api.Features.Register.Model;
using Api.Features.Register.Repository;

namespace Api.Features.Register.Service;

public class RegisterService
{
    private readonly IUserRepository _userRepository;

    public RegisterService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("Email already exists.");

        User user = User.Create(request);

        await _userRepository.AddAsync(user);
    }
}