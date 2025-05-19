using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Plans.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Plans.Handlers.CommandHandlers
{
    public class UpdatePlanStatusCommandHandler(IPlanRepository planRepository, IRequestContextService requestContext)
        : BaseFeature(requestContext) ,IRequestHandler<UpdatePlanStatusCommand, string>
    {
        public async Task<string> Handle(UpdatePlanStatusCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var existingPlan = await planRepository.Get(p => p.Id == request.Id, cancellationToken);

            if (existingPlan is null)
            {
                throw new NotFoundException($"Plan with id {request.Id} not found");
            }

            //EnsureUserHasRoleForThisClient(existingPlan.ClientId);

            if (existingPlan.Status == request.Status)
            {
                throw new BadRequestException($"Updateable plan statuses are same");
            }

            //TODO: [Optimization] we have to avoid double calling of database.
            await planRepository.UpdateSingleField(p => p.Id == request.Id, p => p.Status, request.Status, cancellationToken);
            return existingPlan.Id;
        }
    }
}
