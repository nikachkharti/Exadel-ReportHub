using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Identity.Models;
using System.Threading.Tasks;

namespace ReportHub.Identity.Controllers;

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
    public IActionResult Get() => Ok(_roleManager.Roles.Select(r => r.Name).ToList());

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
}
