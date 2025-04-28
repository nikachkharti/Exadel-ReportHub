using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Identity.Application.Features.Users.Commands;
using ReportHub.Identity.Application.Features.Users.Queries;

namespace ReportHub.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [Authorize(Roles = "SuperAdmin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await mediator.Send(command);

            return CreatedAtAction(nameof(CreateUser), new { id = result });
        }

        [Authorize(Roles = "SuperAdmin, Owner, ClientAdmin",AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
        {
            var result = await mediator.Send(query);

            return Ok(result);
        }
    }
}
