using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Features.Customers.Queries;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace ReportHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetAllCustomers([FromQuery][Required] int? pageNumber = 1, [FromQuery][Required] int? pageSize = 10, [FromQuery] string sortingParameter = "", [FromQuery][Required] bool ascending = true)
        {
            try
            {
                Log.Information("Fetching all customers.");

                var query = new GetAllCustomersQuery(pageNumber, pageSize, sortingParameter, ascending);
                var customers = await mediator.Send(query);

                return Ok(customers);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Get customer by id
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>IActionResult</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleCustomer([FromRoute][Required] string id)
        {
            try
            {
                Log.Information("Getting a single customer.");

                var query = new GetCustomerByIdQuery(id);
                var customer = await mediator.Send(query);

                return Ok(customer);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Add new customer
        /// </summary>
        /// <param name="model">Customer model</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        public async Task<IActionResult> AddNewCustomer([FromBody] CreateCustomerCommand model)
        {
            
            Log.Information("Adding a new customer.");

            var customer = await mediator.Send(model);

            return Ok(customer);
          
            
        }


        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute][Required] string id)
        {
            try
            {
                Log.Information("Deleting a single customer.");

                var command = new DeleteCustomerCommand(id);
                var client = await mediator.Send(command);

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="model">Update document</param>
        /// <returns>IActionResult</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromForm][Required] UpdateCustomerCommand model)
        {
            try
            {
                Log.Information("Updating a single customer.");

                var client = await mediator.Send(model);

                return Ok(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
