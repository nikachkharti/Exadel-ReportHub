using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.API.Authorization.Attributes;
using ReportHub.API.Authorization.Permissions;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.Item.Commands;
using ReportHub.Application.Features.Item.DTOs;
using ReportHub.Application.Features.Item.Queries;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ReportHub.API.Controllers
{
    [Route("api/clients/{clientId}/[controller]")]
    [ApiController]
    public class ItemsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get client item by item id
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetSingleItem(string clientId,[FromRoute][Required] string itemId)
        {
            var query = new GetItemByIdQuery(itemId);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }



        /// <summary>
        /// Create item for client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Permission(PermissionType.CreateItem)]
        [HttpPost]
        public async Task<IActionResult> CreateItem(string clientId, [FromBody] ItemForCreatingDto model)
        {
            Log.Information("Creating a new item.");
            var result = await mediator.Send(new CreateItemCommand(clientId, model.Name, model.Description, model.Price, model.Currency));

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }

    }
}
