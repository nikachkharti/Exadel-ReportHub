﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Clients.Queries;
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
    }
}
