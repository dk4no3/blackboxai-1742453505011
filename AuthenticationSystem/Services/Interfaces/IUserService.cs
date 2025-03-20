using AuthenticationSystem.DTOs;
using AuthenticationSystem.Models;

namespace AuthenticationSystem.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(Guid id);
    Task<UserResponse?> GetUserByUsernameAsync(string username);
    Task<bool> UpdateUserAsync(Guid id, UserResponse updatedUser);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> AssignRoleAsync(Guid userId, string roleName);
    Task<bool> RemoveRoleAsync(Guid userId, string roleName);
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
}