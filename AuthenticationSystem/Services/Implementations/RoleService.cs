using Microsoft.EntityFrameworkCore;
using AuthenticationSystem.Data;
using AuthenticationSystem.Models;
using AuthenticationSystem.Services.Interfaces;

namespace AuthenticationSystem.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetRoleByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Role?> GetRoleByNameAsync(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<Role> CreateRoleAsync(string name, string? description)
    {
        var role = new Role
        {
            Name = name,
            Description = description
        };

        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        return role;
    }

    public async Task<bool> UpdateRoleAsync(Guid id, string name, string? description)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return false;

        // Check if the new name is already taken by another role
        if (role.Name != name)
        {
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name);
            if (existingRole != null) return false;
        }

        role.Name = name;
        role.Description = description;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteRoleAsync(Guid id)
    {
        var role = await _context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null) return false;

        // Check if this is a system role (Admin or User)
        if (role.Name == "Admin" || role.Name == "User")
            return false; // Cannot delete system roles

        // Remove the role from all users
        foreach (var user in role.Users.ToList())
        {
            role.Users.Remove(user);
        }

        _context.Roles.Remove(role);

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RoleExistsAsync(string name)
    {
        return await _context.Roles.AnyAsync(r => r.Name == name);
    }

    public async Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName)
    {
        var role = await _context.Roles
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Name == roleName);

        return role?.Users ?? new List<User>();
    }
}