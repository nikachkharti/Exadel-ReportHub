using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ReportHub.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : Controller
{
    [HttpGet]
    public string Get()
    {
        return "Hello from ReportHub API";
    }
}