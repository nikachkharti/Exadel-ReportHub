using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
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
        public async Task<IActionResult> GetUserClients()
        {
            var userId = requestContextService.GetUserId();

            var result = await mediator.Send(new GetUserClientsQuery(userId));

            return Ok(result);
        }
    }
}
