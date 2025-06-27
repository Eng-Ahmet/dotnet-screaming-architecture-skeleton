using Api.Features.Register.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }
}
