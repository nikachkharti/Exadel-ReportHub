using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Plans.Commands
{
    public record UpdatePlanCommand
        (
            string Id,
            string ItemId,
            string ClientId,
            decimal Amount,
            DateTime StartDate,
            DateTime EndDate,
            PlanStatus Status
        ) : IRequest<string>;
}
