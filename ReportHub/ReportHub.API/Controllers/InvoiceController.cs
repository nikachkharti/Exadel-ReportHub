using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.API.Enums;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.DataExports.Queries;
using ReportHub.Application.Features.DataExports.Queries.CsvQueries;
using ReportHub.Application.Features.DataExports.Queries.ExcelQueries;
using ReportHub.Application.Features.DataExports.Queries.PdfQueries;
using ReportHub.Application.Features.InvoiceLogs.Queries;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Invoices.Queries;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ReportHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController(IMediator _mediator) : ControllerBase
    {

        /// <summary>
        /// Get invoices in a date range, can be filtered by client or customer id
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="clientId">Client id</param>
        /// <param name="customerId">Customer id</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>IActionResult</returns>
        [HttpGet("date")]
        public async Task<IActionResult> GetAllInvoicesInADateRange(
            [FromQuery][Required] DateTime startDate,
            [FromQuery][Required] DateTime endDate,
            [FromQuery] string clientId = "",
            [FromQuery] string customerId = "",
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string sortingParameter = "",
            [FromQuery] bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllInvoicesInADateRangeQuery
            (
                startDate,
                endDate,
                clientId,
                customerId,
                pageNumber,
                pageSize,
                sortingParameter,
                ascending,
                cancellationToken
            );

            var result = await _mediator.Send(query, cancellationToken);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Getting all invoices from database
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [Authorize(Roles = "Owner, ClientAdmin,Operator")]
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
        [Authorize(Roles = "Owner, ClientAdmin,Operator")]
        public async Task<ActionResult<InvoiceForGettingDto>> GetById([FromRoute] string id)
        {
            var result = await _mediator.Send(new GetInvoicesByIdQuery(id));
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        ///// <summary>
        ///// Reads file and imports invoices to database if not exist
        ///// </summary>
        ///// <param name="fileType"></param>
        ///// <param name="file"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //[HttpPost("import")]
        //public async Task<IActionResult> Import([FromQuery] FileImportingType fileType, IFormFile file, CancellationToken cancellationToken)
        //{
        //    var query = GetImportingQuery(fileType, file.OpenReadStream(), Path.GetExtension(file.FileName));
        //    var result = await _mediator.Send(query, cancellationToken);

        //    var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
        //    return StatusCode(response.HttpStatusCode, response);
        //}


        /// <summary>
        /// Exports invoices to file type user chose
        /// </summary>
        /// <param name="fileType">0-CSV  1-Excel  2-PDF</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("export")]
        [Authorize(Roles = "Owner, ClientAdmin,Operator")]
        public async Task<IActionResult> Export([FromQuery] FileExportingType fileType, CancellationToken cancellationToken)
        {
            var query = GetExportingQuery(fileType);
            var result = await _mediator.Send(query, cancellationToken);
            return File(result, "application/octet-stream", $"Invoices{query.Extension}");
        }



        /// <summary>
        /// Exports specific invoice by id to file type user chose
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <param name="fileType">Export file type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File result</returns>
        [HttpGet("{id}/export")]
        [Authorize(Roles = "Owner, ClientAdmin,Operator")]
        public async Task<IActionResult> ExportById([FromRoute] string id, [FromQuery] FileExportingType fileType, CancellationToken cancellationToken)
        {
            var query = GetExportingQueryById(id, fileType);
            var result = await _mediator.Send(query, cancellationToken);
            return File(result, "application/octet-stream", $"Invoice_{id}{query.Extension}");
        }


        /// <summary>
        /// Export invoice logs in a specific date range
        /// </summary>
        /// <param name="startDate">From log create date</param>
        /// <param name="endDate">To log create date</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>IActionResult</returns>
        [HttpGet("logs")]
        public async Task<IActionResult> ExportLogsInDateRange
            ([FromQuery][Required] DateTime startDate,
            [FromQuery][Required] DateTime endDate,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string sortingParameter = "",
            [FromQuery] bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllInvoiceLogsInDateRangeQuery(startDate, endDate, pageNumber, pageSize, sortingParameter, ascending, cancellationToken);
            var result = await _mediator.Send(query);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }



        /// <summary>
        /// Export invoice logs of specific user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>IActionResult</returns>
        [HttpGet("logs/user/{userId}")]
        [Authorize(Roles = "Owner, ClientAdmin,Operator,SuperAdmin")]
        public async Task<IActionResult> ExportLogsOfUser
            ([FromRoute] string userId,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string sortingParameter = "",
            [FromQuery] bool ascending = true,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllInvoiceLogsOfUserQuery(userId, pageNumber, pageSize, sortingParameter, ascending, cancellationToken);
            var result = await _mediator.Send(query);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        [Authorize]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] GetStatistcsQuery query)
        {
            var result = await _mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

        private ExportBaseQuery GetExportingQueryById(string id, FileExportingType fileType)
        {
            return fileType switch
            {
                FileExportingType.Pdf => new InvoiceExportByIdAsPdfQuery(id),
                FileExportingType.Excel => new InvoiceExportByIdAsExcelQuery(id),
                FileExportingType.Csv => new InvoiceExportByIdAsCsvQuery(id),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }

        //private ImportBaseQuery GetImportingQuery(FileImportingType fileType, Stream stream, string extension)
        //{
        //    return fileType switch
        //    {
        //        FileImportingType.CSV => new InvoiceImportAsCsvQuery(stream, extension),
        //        FileImportingType.Excel => new InvoiceImportAsExcelQuery(stream, extension),
        //        _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
        //    };
        //}
        private ExportBaseQuery GetExportingQuery(FileExportingType fileType)
        {
            return fileType switch
            {
                FileExportingType.Csv => new InvoiceExportAsCsvQuery(),
                FileExportingType.Excel => new InvoiceExportAsExcelQuery(),
                FileExportingType.Pdf => new InvoiceExportAsPdfQuery(),
                _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null)
            };
        }

    }
}
