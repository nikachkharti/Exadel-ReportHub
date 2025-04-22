using MediatR;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Plans.Commands
{
    public record UpdatePlanStatusCommand(string Id, PlanStatus Status) : IRequest<string>;
}
