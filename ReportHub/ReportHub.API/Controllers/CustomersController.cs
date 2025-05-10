using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.API.Authorization.Attributes;
using ReportHub.API.Authorization.Permissions;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Features.Customers.DTOs;
using ReportHub.Application.Features.Customers.Queries;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ReportHub.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Get all customers
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortingParameter"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Owner, ClientAdmin, Operator")]
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Owner, ClientAdmin, Operator")]
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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Owner, ClientAdmin, Operator")]
        public async Task<IActionResult> AddNewCustomer([FromBody] CustomerForCreatingDto model)
        {
            var result = await mediator.Send(new CreateCustomerCommand(model.Name, model.Email, model.CountryId));
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Owner,ClientAdmin")]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}
