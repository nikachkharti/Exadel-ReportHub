using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Identity.Features.Users.Commands;

namespace ReportHub.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await mediator.Send(command);

            return CreatedAtAction(nameof(CreateUser), new { id = result });
        }
    }
}
