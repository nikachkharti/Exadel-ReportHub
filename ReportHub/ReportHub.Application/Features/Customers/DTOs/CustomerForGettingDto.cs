namespace ReportHub.Application.Features.Customers.DTOs
{
    public record CustomerForGettingDto
    (
        string Id,
        string Name,
        string Email,
        string CountryId
    );
}
