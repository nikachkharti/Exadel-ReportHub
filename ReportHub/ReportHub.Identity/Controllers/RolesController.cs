using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Identity.Domain.Entities;

namespace ReportHub.Identity.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly RoleManager<Role> _roleManager;

    public RolesController(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var roles = _roleManager.Roles.ToList();

        if(roles is null || roles.Count == 0)
        {
            return NoContent();
        }

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return NotFound("Role not found");

        return Ok(role);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleCreation newRole)
    {
        if (string.IsNullOrWhiteSpace(newRole.Name))
            return BadRequest("Role name cannot be empty.");

        var result = await _roleManager.CreateAsync(new Role(newRole.Name));

        if (result.Succeeded)
            return Created();

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is null)
            return NotFound("Role not found");

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
            return NoContent();

        return BadRequest(result.Errors);
    }
}
