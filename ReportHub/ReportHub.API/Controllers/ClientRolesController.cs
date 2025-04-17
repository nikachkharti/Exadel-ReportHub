using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.ClientRoles.Commands;
using ReportHub.Application.Features.ClientRoles.Queries;

namespace ReportHub.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientRolesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken) 
        {
            var clientRoles = await mediator.Send(new GetAllClientRolesQuery(), cancellationToken);

            if(clientRoles is null || !clientRoles.Any())
            {
                return NoContent();
            }

            return Ok(clientRoles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientRoleCommand command, CancellationToken cancellationToken)
        {
            var clientRole = await mediator.Send(command, cancellationToken);

            return Ok(clientRole);
        }
    }
}
