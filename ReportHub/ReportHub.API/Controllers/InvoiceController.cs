using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.API.Enums;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.DataExports.Queries;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;
using ReportHub.Application.Features.DataExports.Queries.ExcelQueries;
using ReportHub.Application.Features.DataImports.Queries;
using ReportHub.Application.Features.DataImports.Queries.CsvQueries;
using ReportHub.Application.Features.DataImports.Queries.ExcelQueries;
using ReportHub.Application.Features.Invoices.Commands;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Invoices.Queries;
using Serilog;
using System.Net;

namespace ReportHub.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "SuperAdmin, Admin, ClientAdmin")]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceCommand createInvoiceCommand)
        {
            return Ok(await _mediator.Send(createInvoiceCommand));
        }

        /// <summary>
        /// Getting all invoices from database
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var query = new GetAllInvoicesQuery();
            var result = await _mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Getting invoice from database by Id
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceForGettingDto>> GetById([FromRoute] string id)
        {
            var result = await _mediator.Send(new GetInvoicesByIdQuery(id));
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Reads file and imports invoices to database if not exist
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromQuery] FileImportingType fileType, IFormFile file, CancellationToken cancellationToken)
        {
            var query = GetImportingQuery(fileType, file.OpenReadStream(), Path.GetExtension(file.FileName));
            var result = await _mediator.Send(query, cancellationToken);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
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
            var result = await _mediator.Send(query, cancellationToken);
            return File(result, "application/octet-stream", $"Invoices{query.Extension}");
        }


        private ImportBaseQuery GetImportingQuery(FileImportingType fileType, Stream stream, string extension)
        {
            return fileType switch
            {
                FileImportingType.CSV => new InvoiceImportAsCsvQuery(stream, extension),
                FileImportingType.Excel => new InvoiceImportAsExcelQuery(stream, extension),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }
        private ExportBaseQuery GetExportingQuery(FileExportingType fileType)
        {
            return fileType switch
            {
                FileExportingType.Csv => new InvoiceExportAsCsvQuery(),
                FileExportingType.Excel => new InvoiceExportAsExcelQuery(),
                FileExportingType.Pdf => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }

    }
}
