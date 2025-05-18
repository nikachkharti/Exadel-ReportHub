using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Plans.DTOs;
using ReportHub.Application.Features.Plans.Queries;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Plans.Handlers.QueryHandlers
{
    public class GetPlanByIdQueryHandler(IPlanRepository planRepository, IMapper mapper, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<GetPlanByIdQuery, PlanForGettingDto>
    {
        public async Task<PlanForGettingDto> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            var plan = await planRepository.Get(p => p.Id == request.Id, cancellationToken);


            if (plan is null)
            {
                throw new NotFoundException($"Plan with id {request.Id} not found");
            }

            //EnsureUserHasRoleForThisClient(plan.ClientId);

            return mapper.Map<PlanForGettingDto>(plan);
        }
    }
}
