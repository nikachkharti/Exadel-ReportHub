using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Application.Features.CLientUsers.Queries;
using ReportHub.Application.Features.Items.Commands;
using System.ComponentModel.DataAnnotations;
using ReportHub.Application.Common.Models;
using System.Net;
using ReportHub.Application.Features.Plans.Queries;
using ReportHub.Application.Features.Sale.Queries;
using ReportHub.Application.Features.Clients.DTOs;

namespace ReportHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get all clients
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Authorize(Roles = "Owner, ClientAdmin,Operator,SuperAdmin")]
        public async Task<IActionResult> GetAllClients([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string sortingParameter = "", [FromQuery] bool ascending = true)
        {
            var query = new GetAllClientsQuery(pageNumber, pageSize, sortingParameter, ascending);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Get client by id
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{clientId}")]
        [Authorize(Roles = "Owner, ClientAdmin,Operator,SuperAdmin")]
        public async Task<IActionResult> GetSingleClient([FromRoute][Required] string clientId)
        {
            var query = new GetClientByIdQuery(clientId);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get all items of client
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{clientId}/items")]
        [Authorize(Roles = "Owner, ClientAdmin,Operator")]
        public async Task<IActionResult> GetAllItemsOfClient(
           [FromRoute][Required] string clientId,
           [FromQuery] int? pageNumber = 1,
           [FromQuery] int? pageSize = 10,
           [FromQuery] string sortingParameter = "",
           [FromQuery] bool ascending = true
        )
        {
            var query = new GetAllItemsOfClientQuery(clientId, pageNumber, pageSize, sortingParameter, ascending);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Add new client
        /// </summary>
        /// <param name="model">Client model</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> AddNewClient([FromBody] CreateClientCommand model)
        {
            var result = await mediator.Send(model);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Delete client
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{clientId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteClient([FromRoute][Required] string clientId)
        {
            var command = new DeleteClientCommand(clientId);
            var result = await mediator.Send(command);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.NoContent));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Delete single item of client
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <param name="itemId">Item Id</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{clientId}/items/{itemId}")]
        [Authorize(Roles = "Owner, ClientAdmin")]
        public async Task<IActionResult> DeleteItem([FromRoute][Required] string clientId, [FromRoute][Required] string itemId)
        {
            var query = new DeleteItemOfClientCommand(clientId, itemId);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.NoContent));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Update client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Owner, SuperAdmin")]
        public async Task<IActionResult> UpdateClient([FromBody][Required] UpdateClientCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get all plans of client
        /// </summary>
        /// <param name="clientId">Client id</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <returns>IActionResult</returns>
        [Authorize(Roles = "Owner, ClientAdmin")]
        [HttpGet("{clientId}/plans")]
        public async Task<IActionResult> GetAllPlansOfClient(
            [FromRoute][Required] string clientId,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string sortingParameter = "",
            [FromQuery] bool ascending = true)
        {
            var query = new GetPlansOfClientQuery(clientId, pageNumber, pageSize, sortingParameter, ascending);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get all sells of client
        /// </summary>
        /// <param name="clientId">Client id</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <returns>IActionResult</returns>
        /// <returns>IActionResult</returns>
        [HttpGet("{clientId}/sales")]
        public async Task<IActionResult> GetAllSalesOfClient(
            [FromRoute][Required] string clientId,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string sortingParameter = "",
            [FromQuery] bool ascending = true)
        {
            var query = new GetSalesByClientIdQuery(clientId, pageNumber, pageSize, sortingParameter, ascending);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Selling of item
        /// </summary>
        /// <param name="model">Selling item model</param>
        /// <returns>IActionResult</returns>
        [HttpPost("sell")]
        public async Task<IActionResult> SellItem([FromBody][Required] SellItemCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        [HttpGet("revenue")]
        [Authorize]
        public async Task<IActionResult> GetRevenue([FromQuery] GetRevenueInRangeQuery query)
        {
            var result = await mediator.Send(query);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
