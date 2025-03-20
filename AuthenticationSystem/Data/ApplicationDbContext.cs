using Microsoft.EntityFrameworkCore;
using AuthenticationSystem.Models;

namespace AuthenticationSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure many-to-many relationship between Users and Roles
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users);

        // Add unique constraint on Username
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // Add unique constraint on Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Seed initial roles
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Description = "Administrator role with full access"
            },
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "User",
                Description = "Standard user role"
            }
        );
    }
}