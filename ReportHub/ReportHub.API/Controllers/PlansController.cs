using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Common.Models;
using ReportHub.Application.Features.Plans.Commands;
using System.Net;

namespace ReportHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Add new plan
        /// </summary>
        /// <param name="model">New plan model</param>
        /// <returns>IActionResult</returns>
        [HttpPost]
        [Authorize(Roles = "Owner, ClientAdmin,SuperAdmin")]
        public async Task<IActionResult> AddNewPlan([FromBody] CreatePlanCommand model)
        {
            var result = await mediator.Send(model);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Update plan
        /// </summary>
        /// <param name="model">Update plan model</param>
        /// <returns>IActionResult</returns>
        [HttpPut]
        [Authorize(Roles = "Owner, ClientAdmin,SuperAdmin")]
        public async Task<IActionResult> UpdatePlan([FromBody] UpdatePlanCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Delete plan
        /// </summary>
        /// <param name="id">Plan id</param>
        /// <returns>IActionResult</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeletePlan([FromRoute] string id)
        {
            var query = new DeletePlanCommand(id);
            var result = await mediator.Send(query);

            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.NoContent));
            return StatusCode(response.HttpStatusCode, response);
        }


        /// <summary>
        /// Update status of plan manually
        /// </summary>
        /// <param name="model">Status update model</param>
        /// <returns>IActionResult</returns>
        [HttpPatch("status")]
        [Authorize(Roles = "Owner, ClientAdmin,SuperAdmin")]
        public async Task<IActionResult> ChangePlanStatus([FromBody] UpdatePlanStatusCommand model)
        {
            var result = await mediator.Send(model);
            var response = new EndpointResponse(result, EndpointMessage.successMessage, isSuccess: true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(response.HttpStatusCode, response);
        }

    }


}
