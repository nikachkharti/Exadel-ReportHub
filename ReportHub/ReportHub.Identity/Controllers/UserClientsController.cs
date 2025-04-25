using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Identity.Application.Features.UserClients.Queries;

namespace ReportHub.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClientsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("/{userId}/clients")]
        public async Task<IActionResult> GetUserClients(string userId)
        {
            var result = await mediator.Send(new GetUserClientsQuery(userId));

            return Ok(result);
        }

        [HttpGet("/{clientId}/users")]
        public async Task<IActionResult> GetClientUsers(string clientId)
        {
            var result = await mediator.Send(new GetClientUsersQuery(clientId));

            return Ok(result);
        }
    }
}
