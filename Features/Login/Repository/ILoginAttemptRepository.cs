using Api.Features.Login.Model;

namespace Api.Features.Login.Repository;

public interface ILoginAttemptRepository
{
    Task AddAsync(LoginAttempt attempt);
    Task<IEnumerable<LoginAttempt>> GetByUserIdAsync(int userId);
    Task<IEnumerable<LoginAttempt>> GetFailedAttemptsByUserIdAsync(int userId);
}
