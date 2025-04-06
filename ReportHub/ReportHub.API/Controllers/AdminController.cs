using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace ReportHub.API.Controllers;

[Authorize(Roles = "admin",
    AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AdminController
{
    [HttpPost("change-user-role")]
    public string ChangeUserRole()
    {
        return "Admin changing user role";
    }
}