using Microsoft.AspNetCore.Mvc;
using MediatR;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Contracts;

namespace ReportHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllClients()
        {
            var query = new GetAllClientsQuery();
            var clients = await _mediator.Send(query);

            if (clients == null || !clients.Any())
            {
                return NoContent();
            }

            return Ok(clients);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClientDto>> GetClientById(Guid id)
        {
            var query = new GetByIdClientQuery { Id = id };
            var client = await _mediator.Send(query);

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateClient([FromBody] CreateClientCommand command)
        {
            if (command == null)
            {
                return BadRequest("Invalid client data."); 
            }

            var clientId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetClientById), new { id = clientId }, clientId);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateClient(Guid id, [FromBody] UpdateClientCommand command)
        {
            if (command == null)
            {
                return BadRequest("Invalid client data.");
            }
            command.ClientId = id;

            var result = await _mediator.Send(command);
            if (result == Guid.Empty)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteClient(Guid id)
        {
            var command = new DeleteClientCommand { ClientId = id };
            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}