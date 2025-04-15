﻿using MediatR;
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

namespace ReportHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Superadmin, Admin, ClientAdmin")]
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
        public async Task<IActionResult> GetAllClients([FromQuery][Required] int? pageNumber = 1, [FromQuery][Required] int? pageSize = 10, [FromQuery] string sortingParameter = "", [FromQuery][Required] bool ascending = true)
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
        [HttpPost]
        public async Task<IActionResult> AddNewClient([FromForm] CreateClientCommand model)
        {
            var result = await mediator.Send(model);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Add user to client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "SuperAdmin, Admin, ClientAdmin")]
        [HttpPost("{clientId}/users")]
        public async Task<IActionResult> AddUserToClient(string clientId, [FromBody] AddUserToClientDto model)
        {
            var result = await mediator.Send(new AddUserToClientCommand(clientId, model.UserId, model.Role));

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }


        [HttpGet("{clientId}/users")]
        public async Task<IActionResult> GetClientUsersByClientId(string clientId, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetAllClientUserByClientIdQuery(clientId), cancellationToken);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        /// <summary>
        /// Delete client
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>IActionResult</returns>
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
        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromForm][Required] UpdateClientCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

    }
}
