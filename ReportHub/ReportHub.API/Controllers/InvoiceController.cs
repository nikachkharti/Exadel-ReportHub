using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Queries;
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
    }
}
