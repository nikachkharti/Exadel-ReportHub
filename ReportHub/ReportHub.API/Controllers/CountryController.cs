using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReportHub.Application.Contracts.RepositoryContracts;
using System.ComponentModel.DataAnnotations;

namespace ReportHub.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController(ICountryRepository countryRepository, ICurrencyRepository currencyRepository) : ControllerBase
{
    [HttpGet("countries")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await countryRepository.GetAll());
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> Get()
    {
        return Ok(await currencyRepository.GetAll());
    }
}
