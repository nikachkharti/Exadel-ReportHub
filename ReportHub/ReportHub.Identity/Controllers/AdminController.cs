using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Identity.Domain.Entities;

namespace ReportHub.Identity.Controllers;
[Authorize(Roles = "Admin, SystemAdmin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Route("api/[controller]/users")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public AdminController(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.Users.ToList();

        if (users is null || users.Count == 0)
        {
            return NoContent();
        }

        return Ok(users);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("{userId}/roles")]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] AssignRoleRequest request)
    {

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound("User not found");

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);

        if (!roleExists)
            return BadRequest("Role does not exist");

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (result.Succeeded)
            return Ok("Role assigned successfully");

        return BadRequest(result.Errors);
    }

    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<IActionResult> RemoveRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound("User not found");

        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
            return BadRequest("Role does not exist");

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (result.Succeeded)
            return Ok("Role removed successfully");

        return BadRequest(result.Errors);
    }

    [HttpGet("{userId}/roles")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return NotFound("User not found");

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(roles);
    }
}
