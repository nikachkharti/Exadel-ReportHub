using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.ReportSchedule.Commands;
using ReportHub.Application.Features.ReportSchedule.Queries;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ReportHub.API.Controllers
{
    [Route("api/reportschedules")]
    [ApiController]
    public class ReportSchedulesController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Add new report schedule
        /// </summary>
        /// <param name="command">CreateReportScheduleCommand</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateReportScheduleCommand command)
        {
            var result = await mediator.Send(command);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get report schedules of customer
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <returns>IActionResult</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetReportSchedulesOfCustomer
        (
            [FromRoute][Required] string customerId,
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string sortingParameter = "",
            [FromQuery] bool ascending = true
        )
        {
            var query = new GetAllReportSchedulesOfCustomerQuery(customerId, pageNumber, pageSize, sortingParameter, ascending);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get report schedules of customer
        /// </summary>
        /// <param name="reportScheduleId">Report schedule id</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{customerId}")]
        public async Task<IActionResult> DeleteReportSchedule([FromRoute][Required] string reportScheduleId)
        {
            var command = new DeleteReportScheduleCommand(reportScheduleId);
            var result = await mediator.Send(command);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
