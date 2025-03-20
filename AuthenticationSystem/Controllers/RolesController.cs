using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthenticationSystem.Models;
using AuthenticationSystem.Services.Interfaces;

namespace AuthenticationSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Only admins can manage roles
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Role>> GetRoleById(Guid id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<Role>> GetRoleByName(string name)
    {
        var role = await _roleService.GetRoleByNameAsync(name);
        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<Role>> CreateRole([FromBody] RoleCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Role name is required");

        // Check if role already exists
        var existingRole = await _roleService.GetRoleByNameAsync(request.Name);
        if (existingRole != null)
            return BadRequest("Role already exists");

        var role = await _roleService.CreateRoleAsync(request.Name, request.Description);
        return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Role name is required");

        // Prevent updating system roles
        var existingRole = await _roleService.GetRoleByIdAsync(id);
        if (existingRole == null)
            return NotFound();

        if (existingRole.Name is "Admin" or "User")
            return BadRequest("Cannot modify system roles");

        var result = await _roleService.UpdateRoleAsync(id, request.Name, request.Description);
        if (!result)
            return BadRequest("Failed to update role");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        if (!result)
            return BadRequest("Failed to delete role. Make sure it's not a system role and has no users assigned.");

        return NoContent();
    }

    [HttpGet("{roleName}/users")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersInRole(string roleName)
    {
        var users = await _roleService.GetUsersInRoleAsync(roleName);
        return Ok(users);
    }
}

// Request DTOs
public class RoleCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class RoleUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}