using Api.Features.Login.Model;
using Api.Features.Register.Model;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<LoginAttempt> LoginAttempts { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LoginAttempt>()
            .HasOne(a => a.User)
            .WithMany(u => u.LoginAttempts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }
}
