﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Application.Features.CLientUsers.DTOs;
using ReportHub.Application.Features.CLientUsers.Queries;
using Serilog;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> GetAllClients([FromQuery][Required] int? pageNumber = 1, [FromQuery][Required] int? pageSize = 10, [FromQuery] string sortingParameter = "", [FromQuery][Required] bool ascending = true)
        {
            try
            {
                Log.Information("Fetching all clients.");

                var query = new GetAllClientsQuery(pageNumber, pageSize, sortingParameter, ascending);
                var clients = await mediator.Send(query);

                return Ok(clients);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Get client by id
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleClient([FromRoute][Required] string id)
        {
            try
            {
                Log.Information("Getting a single client.");

                var query = new GetClientByIdQuery(id);
                var client = await mediator.Send(query);

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Add new client
        /// </summary>
        /// <param name="model">Client model</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> AddNewClient([FromForm] CreateClientCommand model)
        {
            try
            {
                Log.Information("Adding a new client.");

                var client = await mediator.Send(model);

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add user to client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost("{clientId}/users")]
        public async Task<IActionResult> AddUserToClient(string clientId, [FromBody] AddUserToClientDto model)
        {
            try
            {
                Log.Information("Adding a user to a client.");

                var client = await mediator.Send(new AddUserToClientCommand(clientId, model.UserId, model.Role));

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{clientId}/users")]
        public async Task<IActionResult> GetClientUsersByClientId(string clientId, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Getting all users for a client.");

                var clientUsers = await mediator.Send(new GetAllClientUserByClientIdQuery(clientId), cancellationToken);
                return Ok(clientUsers);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete client
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient([FromRoute][Required] string id)
        {
            try
            {
                Log.Information("Deleting a single client.");

                var command = new DeleteClientCommand(id);
                var client = await mediator.Send(command);

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Update client
        /// </summary>
        /// <param name="model">Update document</param>
        /// <returns>IActionResult</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromForm][Required] UpdateClientCommand model)
        {
            try
            {
                Log.Information("Updating a single client.");

                var client = await mediator.Send(model);

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

    }
}
