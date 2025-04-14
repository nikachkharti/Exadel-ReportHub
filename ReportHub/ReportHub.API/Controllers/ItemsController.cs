using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Item.Commands;
using ReportHub.Application.Features.Item.Queries;
using ReportHub.Application.Validators.Exceptions;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace ReportHub.API.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin, ClientAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get single item by id
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleItem([FromRoute][Required] string id)
        {
            try
            {
                Log.Information("Getting a single item.");

                var query = new GetItemByIdQuery(id);
                var item = await mediator.Send(query);

                return Ok(item);
            }
            catch (InputValidationException ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(400, ex.Errors);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ActionResult([FromBody] CreateItemCommand command)
        {
            try
            {
                Log.Information("Creating a new item.");
                var itemId = await mediator.Send(command);
                return CreatedAtAction(nameof(GetSingleItem), new { id = itemId }, itemId);
            }
            catch (InputValidationException ex)
            {
                Log.Error(ex, ex.Message);
                return BadRequest(new { errorMessage = ex.Message, errors = ex.Errors });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
