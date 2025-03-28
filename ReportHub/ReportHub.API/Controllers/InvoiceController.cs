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
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IMediator mediator, ILogger<InvoiceController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllInvoices()
        {
            try
            {
                _logger.LogInformation("Fetching all invoices.");

                var invoices = _mediator.Send(new GetAllInvoicesQuery()).GetAwaiter().GetResult();

                if (!invoices.Any())
                {
                    _logger.LogWarning("No invoices found.");
                    return NoContent();
                }

                _logger.LogInformation("Successfully retrieved invoices.");
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching invoices.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
