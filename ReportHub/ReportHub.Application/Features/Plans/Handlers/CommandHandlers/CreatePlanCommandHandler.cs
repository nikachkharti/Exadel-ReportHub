using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Plans.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Plans.Handlers.CommandHandlers
{
    public class CreatePlanCommandHandler(IPlanRepository planRepository, IMapper mapper, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<CreatePlanCommand, string>
    {
        public async Task<string> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            EnsureUserHasRoleForThisClient(request.ClientId);

            var plan = mapper.Map<Plan>(request);

            await planRepository.Insert(plan);
            return plan.Id;
        }
    }
}
