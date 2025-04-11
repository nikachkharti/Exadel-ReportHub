using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace ReportHub.API.Controllers;

[Authorize(Roles = "Admin",
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AdminController : Controller
{
    [HttpPost("change-user-role")]
    public string ChangeUserRole()
    {
        return "Admin changing user role";
    }
}