using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using ReportHub.Identity.Application.Features.UserClients.Commands;
using ReportHub.Identity.Application.Features.UserClients.Queries;
using ReportHub.Identity.Application.Interfaces.ServiceInterfaces;

namespace ReportHub.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClientsController(IMediator mediator, IRequestContextService requestContextService) : ControllerBase
    {
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("/my-clients")]
        public async Task<IActionResult> GetNyClients()
        {
            var userId = requestContextService.GetUserId();

            var result = await mediator.Send(new GetUserClientsQuery(userId));

            return Ok(result);
        }

        [Authorize(Roles = "SuperAdmin, Owner, ClientAdmin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserClientCommand command)
        {
            var result = await mediator.Send(command);

            return CreatedAtAction(nameof(Created), );
        }

    }
}
