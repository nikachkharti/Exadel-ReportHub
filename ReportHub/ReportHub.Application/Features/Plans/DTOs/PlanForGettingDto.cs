using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Plans.DTOs
{
    public record PlanForGettingDto
    (
        string Id,
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime StartDate,
        DateTime EndDate,
        PlanStatus Status
    );
}
