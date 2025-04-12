using Microsoft.AspNetCore.Mvc;
using MediatR;
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

            return Ok(clients);
        }
    }
}