using AtanAlir.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtanAlir.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Nickname).IsUnique();
        });

        modelBuilder.Entity<VerificationCode>(entity =>
        {
            entity.HasIndex(e => e.Code);
            entity.HasIndex(e => e.Email);
        });
    }
}