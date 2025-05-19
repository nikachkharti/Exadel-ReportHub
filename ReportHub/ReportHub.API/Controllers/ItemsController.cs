using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.Items.Queries;
using ReportHub.Application.Features.Items.Commands;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net;

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
        [Authorize(Roles = "Owner, ClientAdmin, Operator")]
        public async Task<IActionResult> GetSingleItem([FromRoute][Required] string id)
        {
            var query = new GetItemByIdQuery(id);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Create new item
        /// </summary>
        /// <param name="command">Create item command</param>
        /// <returns>IActionResult</returns>
        /// <returns>IActionResult</returns>
        [Authorize(Roles = "Owner, ClientAdmin, Operator")]
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Owner, ClientAdmin, Operator")]
        public async Task<IActionResult> CreateItem([FromBody] CreateItemCommand command)
        {
            Log.Information("Creating a new item.");
            var result = await mediator.Send(command);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }

        [Authorize(Roles = "Owner, ClientAdmin, Operator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await mediator.Send(new DeleteItemCommand(id));

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.NoContent));

            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
