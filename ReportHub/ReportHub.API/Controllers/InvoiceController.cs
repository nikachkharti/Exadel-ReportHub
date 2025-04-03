using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.API.Enums;
using ReportHub.Application.Features.DataExports.Queries;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;
using ReportHub.Application.Features.DataImports.Queries;
using ReportHub.Application.Features.DataImports.Queries.CsvQueries;
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

        /// <summary>
        /// Reads file and imports invoices to database if not exist
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromQuery] FileImportingType fileType, IFormFile file,CancellationToken cancellationToken)
        {
            var query = GetImportingQuery(fileType, file.OpenReadStream(), Path.GetExtension(file.FileName));

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Exports invoices to file type user chose
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] FileExportingType fileType, CancellationToken cancellationToken)
        {
            var query = GetExportingQuery(fileType);

            var stream = await _mediator.Send(query, cancellationToken);

            return File(stream, "application/octet-stream", $"Invoices{query.Extension}");
        }

        private ImportBaseQuery GetImportingQuery(FileImportingType fileType, Stream stream, string extension)
        {
            return fileType switch
            {
                FileImportingType.CSV => new InvoiceImportAsCsvQuery(stream, extension),
                FileImportingType.Excel => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }
        private ExportBaseQuery GetExportingQuery(FileExportingType fileType)
        {
            return fileType switch
            {
                FileExportingType.Csv => new InvoiceExportAsCsvQuery(),
                FileExportingType.Excel => throw new NotImplementedException(),
                FileExportingType.Pdf => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }

    }
}
