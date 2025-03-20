using System.ComponentModel.DataAnnotations;

namespace AuthenticationSystem.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    // Navigation property for roles (many-to-many relationship)
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}