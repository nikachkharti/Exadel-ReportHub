using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Application.Features.CLientUsers.DTOs;
using ReportHub.Application.Features.CLientUsers.Queries;
using ReportHub.Application.Features.Item.Commands;
using System.ComponentModel.DataAnnotations;
using ReportHub.Application.Common.Models;
using System.Net;
using ReportHub.API.Authorization.Attributes;
using ReportHub.API.Authorization.Permissions;
using ReportHub.Application.Features.Plans.Queries;
using ReportHub.Application.Features.Sale.Queries;

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
        [Permission(PermissionType.GetAllClients)]
        [HttpGet]
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
        /// <param name="id">Client Id</param>
        /// <returns>IActionResult</returns>
        [Permission(PermissionType.GetOneClient)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleClient([FromRoute][Required] string id)
        {
            var query = new GetClientByIdQuery(id);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get all items of client
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>IActionResult</returns>
        [Permission(PermissionType.GetClientItems)]
        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetAllItemsOfClient([FromRoute][Required] string id)
        {
            var query = new GetAllItemsOfClientQuery(id);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Add new client
        /// </summary>
        /// <param name="model">Client model</param>
        /// <returns>IActionResult</returns>
        [Permission(PermissionType.CreateClient)]
        [HttpPost]
        public async Task<IActionResult> AddNewClient([FromForm] CreateClientCommand model)
        {
            var result = await mediator.Send(model);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Add user to client with role
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Permission(PermissionType.AddUserToClient)]
        [HttpPost("{clientId}/users/{userId}/role")]
        public async Task<IActionResult> AddUserToClient(string clientId, string userId, [FromBody] AddUserToClientDto model)
        {
            var result = await mediator.Send(new AddUserToClientCommand(clientId, userId, model.Role));

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Get client users
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Permission(PermissionType.GetClientUsers)]
        [HttpGet("{clientId}/users")]
        public async Task<IActionResult> GetClientUsersByClientId(string clientId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllClientUserByClientIdQuery(clientId), cancellationToken);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        ///  Remove user from client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Permission(PermissionType.RemoveUserFromClient)]
        [HttpDelete("{clientId}/users/{userId}")]
        public async Task<IActionResult> DeleteUserFromClient(string clientId, string userId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new DeleteClientUserCommand(clientId, userId), cancellationToken);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Delete client
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>IActionResult</returns>
        [Permission(PermissionType.DeleteClient)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute][Required] string id)
        {
            var command = new DeleteClientCommand(id);
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
        [Permission(PermissionType.DeleteItem)]
        [HttpDelete("clients/{clientId}/items/{itemId}")]
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
        /// <param name="model">Update document</param>
        /// <returns>IActionResult</returns>
        [Permission(PermissionType.UpdateClient)]
        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromForm][Required] UpdateClientCommand model)
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
    }
}
