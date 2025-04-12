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
            try
            {
                var query = new GetAllClientsQuery();
                var clients = await _mediator.Send(query);

                if (clients == null || !clients.Any())
                {
                    return NoContent(); 
                }

                return Ok(clients);
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Log.Error(ex, "Error fetching all clients");
                return StatusCode(500, "An error occurred while fetching clients.");
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClientDto>> GetClientById(Guid id)
        {
            try
            {
                var query = new GetByIdClientQuery { Id = id };
                var client = await _mediator.Send(query);

                if (client == null)
                {
                    return NotFound(); // 404 Not Found
                }

                return Ok(client);
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Log.Error(ex, $"Error fetching client by ID: {id}");
                return StatusCode(500, $"An error occurred while fetching client with ID: {id}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateClient([FromBody] CreateClientCommand command)
        {
            try
            {
                if (command == null)
                {
                    return BadRequest("Invalid client data."); // 400 Bad Request
                }

                var clientId = await _mediator.Send(command);

                return CreatedAtAction(nameof(GetClientById), new { id = clientId }, clientId);
            }
            catch (Exception ex)
            {
                // Log the exception details
                // Log.Error(ex, "Error creating client");
                return StatusCode(500, "An error occurred while creating the client.");
            }
        }
    }
}