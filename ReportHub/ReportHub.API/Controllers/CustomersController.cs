using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Features.Customers.Queries;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ReportHub.API.Controllers
{
    [Route("api/clients/{clientId}/[controller]")]
    [ApiController]
    public class CustomersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get all customers
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortingParameter"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers(string clientId,[FromQuery] int? pageNumber = 1, [FromQuery] int? pageSize = 10, [FromQuery] string sortingParameter = "", [FromQuery] bool ascending = true)
        {
            var query = new GetAllCustomersQuery(clientId, pageNumber, pageSize, sortingParameter, ascending);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Get customer by id
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleCustomer(string clientId ,[FromRoute][Required] string id)
        {
            var query = new GetCustomerByIdQuery(id);
            var result = await mediator.Send(query);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Add new customer
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<IActionResult> AddNewCustomer(string clientId,[FromBody] CreateCustomerCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string clientId,[FromRoute][Required] string id)
        {
            var command = new DeleteCustomerCommand(id);
            var result = await mediator.Send(command);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.NoContent));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(string clientId, [FromBody] UpdateCustomerCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
