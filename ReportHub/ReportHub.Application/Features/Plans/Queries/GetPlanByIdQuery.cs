using MediatR;
using ReportHub.Application.Features.Plans.DTOs;

namespace ReportHub.Application.Features.Plans.Queries
{
    public record GetPlanByIdQuery(string Id) : IRequest<PlanForGettingDto>;
}
