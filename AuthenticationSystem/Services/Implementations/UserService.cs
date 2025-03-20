using Microsoft.EntityFrameworkCore;
using AuthenticationSystem.Data;
using AuthenticationSystem.DTOs;
using AuthenticationSystem.Models;
using AuthenticationSystem.Services.Interfaces;

namespace AuthenticationSystem.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IRoleService _roleService;

    public UserService(ApplicationDbContext context, IRoleService roleService)
    {
        _context = context;
        _roleService = roleService;
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.Roles)
            .ToListAsync();

        return users.Select(MapToUserResponse);
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user != null ? MapToUserResponse(user) : null;
    }

    public async Task<UserResponse?> GetUserByUsernameAsync(string username)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == username);

        return user != null ? MapToUserResponse(user) : null;
    }

    public async Task<bool> UpdateUserAsync(Guid id, UserResponse updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        // Check if new username is already taken by another user
        if (user.Username != updatedUser.Username)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == updatedUser.Username);
            if (existingUser != null) return false;
        }

        // Check if new email is already taken by another user
        if (user.Email != updatedUser.Email)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == updatedUser.Email);
            if (existingUser != null) return false;
        }

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;

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

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);

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

    public async Task<bool> AssignRoleAsync(Guid userId, string roleName)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        var role = await _roleService.GetRoleByNameAsync(roleName);
        if (role == null) return false;

        if (user.Roles.Any(r => r.Name == roleName))
            return true; // User already has this role

        user.Roles.Add(role);

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

    public async Task<bool> RemoveRoleAsync(Guid userId, string roleName)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        var role = user.Roles.FirstOrDefault(r => r.Name == roleName);
        if (role == null) return false;

        user.Roles.Remove(role);

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

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Roles.Select(r => r.Name) ?? new List<string>();
    }

    private static UserResponse MapToUserResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = user.Roles.Select(r => r.Name).ToList()
        };
    }
}