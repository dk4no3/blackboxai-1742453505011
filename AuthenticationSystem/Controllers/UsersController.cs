using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthenticationSystem.DTOs;
using AuthenticationSystem.Services.Interfaces;

namespace AuthenticationSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires authentication for all endpoints
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")] // Only admins can get all users
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        // Only allow users to access their own data unless they're an admin
        if (!User.IsInRole("Admin") && User.Identity?.Name != user.Username)
            return Forbid();

        return Ok(user);
    }

    [HttpGet("username/{username}")]
    [Authorize(Roles = "Admin")] // Only admins can lookup users by username
    public async Task<ActionResult<UserResponse>> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserResponse updatedUser)
    {
        // Only allow users to update their own data unless they're an admin
        var currentUser = await _userService.GetUserByUsernameAsync(User.Identity?.Name ?? string.Empty);
        if (currentUser == null)
            return Unauthorized();

        if (!User.IsInRole("Admin") && currentUser.Id != id)
            return Forbid();

        var result = await _userService.UpdateUserAsync(id, updatedUser);
        if (!result)
            return BadRequest("Failed to update user");

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can delete users
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{userId}/roles/{roleName}")]
    [Authorize(Roles = "Admin")] // Only admins can assign roles
    public async Task<IActionResult> AssignRole(Guid userId, string roleName)
    {
        var result = await _userService.AssignRoleAsync(userId, roleName);
        if (!result)
            return BadRequest("Failed to assign role");

        return NoContent();
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    [Authorize(Roles = "Admin")] // Only admins can remove roles
    public async Task<IActionResult> RemoveRole(Guid userId, string roleName)
    {
        // Prevent removing the last admin role
        if (roleName.ToLower() == "admin")
        {
            var userRoles = await _userService.GetUserRolesAsync(userId);
            if (userRoles.Count(r => r.ToLower() == "admin") <= 1)
                return BadRequest("Cannot remove the last admin role");
        }

        var result = await _userService.RemoveRoleAsync(userId, roleName);
        if (!result)
            return BadRequest("Failed to remove role");

        return NoContent();
    }

    [HttpGet("{userId}/roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(Guid userId)
    {
        // Only allow users to view their own roles unless they're an admin
        var currentUser = await _userService.GetUserByUsernameAsync(User.Identity?.Name ?? string.Empty);
        if (currentUser == null)
            return Unauthorized();

        if (!User.IsInRole("Admin") && currentUser.Id != userId)
            return Forbid();

        var roles = await _userService.GetUserRolesAsync(userId);
        return Ok(roles);
    }
}