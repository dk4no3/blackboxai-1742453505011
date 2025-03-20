using System.ComponentModel.DataAnnotations;

namespace AuthenticationSystem.Models;

public class Role
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    // Navigation property for users (many-to-many relationship)
    public ICollection<User> Users { get; set; } = new List<User>();
}