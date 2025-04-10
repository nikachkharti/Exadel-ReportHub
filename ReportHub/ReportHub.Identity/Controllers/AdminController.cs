using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Controllers;

[Route("api/[controller]")]
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
    [HttpGet("users")]
    public IActionResult GetAllUsers() => Ok(_userManager.Users.ToList());

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {

        var user = await _userManager.FindByNameAsync(request.Username);

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
}
