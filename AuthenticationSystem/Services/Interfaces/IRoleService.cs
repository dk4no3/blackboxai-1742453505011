using AuthenticationSystem.Models;

namespace AuthenticationSystem.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(Guid id);
    Task<Role?> GetRoleByNameAsync(string name);
    Task<Role> CreateRoleAsync(string name, string? description);
    Task<bool> UpdateRoleAsync(Guid id, string name, string? description);
    Task<bool> DeleteRoleAsync(Guid id);
    Task<bool> RoleExistsAsync(string name);
    Task<IEnumerable<User>> GetUsersInRoleAsync(string roleName);
}