using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Queries;

namespace ReportHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public InvoiceController(IMediator mediator, Serilog.ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                _logger.Information("Fetching all invoices.");

                var query = new GetAllInvoicesQuery();
                var invoices = await _mediator.Send(query);

                if (!invoices.Any())
                {
                    _logger.Warning("No invoices found.");
                    return NoContent();
                }

                _logger.Information("Successfully retrieved invoices.");
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while fetching invoices.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
