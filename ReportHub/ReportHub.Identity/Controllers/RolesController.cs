using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Identity.Application.Features.Roles.Queries;

namespace ReportHub.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IMediator mediator) : ControllerBase
    {
        //[Authorize(Roles = "SuperAdmin, Owner, ClientAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await mediator.Send(new GetAllRolesQuery());

            return Ok(roles);
        }
    }
}
