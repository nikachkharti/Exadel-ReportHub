﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Invoices.Queries;
using Serilog;

namespace ReportHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Getting all invoices from database
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                Log.Information("Fetching all invoices.");

                var query = new GetAllInvoicesQuery();
                var invoices = await _mediator.Send(query);

                if (!invoices.Any())
                {
                    Log.Warning("No invoices found.");
                    return NoContent();
                }

                Log.Information("Successfully retrieved invoices.");
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching invoices.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Getting invoice from database by Id
        /// </summary>
        /// <returns>IActionResult</returns>

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetById([FromRoute] string id)
        {
            try
            {
                Log.Information($"Fetching Invoice by Id -> {id}");

                var invoice = await _mediator.Send(new GetInvoicesByIdQuery(id));

                if (invoice == null)
                {
                    Log.Warning($"No invoice found for Id -> {id}");
                    return NoContent();
                }

                Log.Information($"Successfully retrieved invoice for Id -> {id}");
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while fetching invoice for Id -> {id}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

    }
}
