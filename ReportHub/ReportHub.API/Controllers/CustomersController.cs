using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Features.Customers.Queries;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ReportHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "SuperAdmin, Admin, ClientAdmin")]
    public class CustomersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get all customers
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortingParameter">Sorting field</param>
        /// <param name="ascending">Is ascended</param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string sortingParameter = "", [FromQuery] bool ascending = true)
        {
            var query = new GetAllCustomersQuery(pageNumber, pageSize, sortingParameter, ascending);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get customer by id
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleCustomer([FromRoute][Required] string id)
        {
            var query = new GetCustomerByIdQuery(id);
            var result = await mediator.Send(query);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Add new customer
        /// </summary>
        /// <param name="model">Customer model</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> AddNewCustomer([FromBody] CreateCustomerCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute][Required] string id)
        {
            var command = new DeleteCustomerCommand(id);
            var result = await mediator.Send(command);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.NoContent));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="model">Update document</param>
        /// <returns>IActionResult</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
