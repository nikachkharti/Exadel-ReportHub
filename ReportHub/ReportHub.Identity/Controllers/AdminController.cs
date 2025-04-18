﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Controllers;
[Authorize(Roles = "Admin, SuperAdmin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
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

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetAllUsers(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if(user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {

        var user = await _userManager.FindByIdAsync(request.UserId);

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

    [HttpDelete("remove-role")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
            return NotFound("User not found");

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);

        if (!roleExists)
            return BadRequest("Role does not exist");

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);

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
