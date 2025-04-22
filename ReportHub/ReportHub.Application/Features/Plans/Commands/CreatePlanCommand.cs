using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Plans.Commands
{
    public record CreatePlanCommand
    (
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime StartDate,
        DateTime EndDate,
        PlanStatus Status
    ) : IRequest<string>;
}
