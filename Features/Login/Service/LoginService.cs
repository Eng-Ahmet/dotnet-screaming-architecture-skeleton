using Api.Features.Login.Model;
using Api.Features.Login.Repository;
using Api.Features.Register.Model;
using Api.Features.Register.Repository;
using Api.Infrastructure.Enums;
using Api.Utils;
using Features.Login.DTO;

namespace Features.Login.Service;

public class LoginService
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginAttemptRepository _loginAttemptRepository;

    public LoginService(IUserRepository userRepository, ILoginAttemptRepository loginAttemptRepository)
    {
        _userRepository = userRepository;
        _loginAttemptRepository = loginAttemptRepository;
    }

    public async Task<User> LoginAsync(LoginRequest request, string ipAddress)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            // تسجيل محاولة خاطئة بدون معرف مستخدم معروف
            await _loginAttemptRepository.AddAsync(new LoginAttempt
            {
                UserId = 0,
                IpAddress = ipAddress,
                IsSuccessful = false,
                FailureReason = "Invalid email"
            });

            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // جلب المحاولات الفاشلة السابقة
        var failedAttempts = await _loginAttemptRepository.GetFailedAttemptsByUserIdAsync(user.Id);
        if (failedAttempts.Count() >= 5)
        {
            user.Status = AccountStatus.Suspended;
            await _userRepository.UpdateAsync(user);

            LoginAttempt attempt = new LoginAttempt
            {
                UserId = user.Id,
                IpAddress = ipAddress,
                IsSuccessful = false,
                FailureReason = "Account suspended due to multiple failed login attempts."
            };
            await _loginAttemptRepository.AddAsync(attempt);

            throw new UnauthorizedAccessException("Your account has been suspended due to multiple failed login attempts.");
        }

        if (!HashHelper.VerifyPassword(request.Password, user.Password))
        {
            await _loginAttemptRepository.AddAsync(new LoginAttempt
            {
                UserId = user.Id,
                IpAddress = ipAddress,
                IsSuccessful = false,
                FailureReason = "Invalid password"
            });

            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (user.Status != AccountStatus.Active)
        {
            await _loginAttemptRepository.AddAsync(new LoginAttempt
            {
                UserId = user.Id,
                IpAddress = ipAddress,
                IsSuccessful = false,
                FailureReason = "Account not active"
            });

            throw new UnauthorizedAccessException("Account is not active.");
        }

        // إذا نجح تسجيل الدخول، نسجل المحاولة الناجحة
        await _loginAttemptRepository.AddAsync(new LoginAttempt
        {
            UserId = user.Id,
            IpAddress = ipAddress,
            IsSuccessful = true
        });

        return user;
    }
}
