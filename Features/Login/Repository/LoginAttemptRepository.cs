using Api.Features.Login.Model;
using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Login.Repository;

public class LoginAttemptRepository : ILoginAttemptRepository
{
    private readonly AppDbContext _context;

    public LoginAttemptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LoginAttempt attempt)
    {
        _context.LoginAttempts.Add(attempt);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LoginAttempt>> GetByUserIdAsync(int userId)
    {
        return await _context.LoginAttempts
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AttemptedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<LoginAttempt>> GetFailedAttemptsByUserIdAsync(int userId)
    {
        var tenMinutesAgo = DateTime.UtcNow.AddMinutes(-10);

        return await _context.LoginAttempts
            .Where(a => a.UserId == userId && !a.IsSuccessful && a.AttemptedAt >= tenMinutesAgo)
            .OrderByDescending(a => a.AttemptedAt)
            .ToListAsync();
    }

}
