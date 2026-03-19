using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Admin;
using Picker.Infrastructure.Identity;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public AdminController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>List all registered users with their roles.</summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserInfoDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? "User",
                CreatedAt = user.CreatedAt
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Assign a role to a user. Valid values: "Admin", "User".
    /// Use this to promote a regular user to Admin or demote an Admin to User.
    /// </summary>
    [HttpPost("users/{userId}/assign-role")]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] AssignRoleDto dto)
    {
        var validRoles = new[] { "Admin", "User" };
        if (!validRoles.Contains(dto.Role, StringComparer.OrdinalIgnoreCase))
            throw new BadRequestException($"Invalid role. Must be one of: {string.Join(", ", validRoles)}");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, dto.Role);

        return Ok(new { message = $"User {user.Email} is now '{dto.Role}'." });
    }
}
