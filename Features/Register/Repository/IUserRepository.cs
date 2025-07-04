
using Api.Features.Register.Model;

namespace Api.Features.Register.Repository;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
}