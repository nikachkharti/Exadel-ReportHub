using MediatR;

namespace ReportHub.Application.Features.Plans.Commands
{
    public record DeletePlanCommand(string Id) : IRequest<string>;
}
