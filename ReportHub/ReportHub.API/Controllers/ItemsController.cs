using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Item.Queries;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace ReportHub.API.Controllers
{
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
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
