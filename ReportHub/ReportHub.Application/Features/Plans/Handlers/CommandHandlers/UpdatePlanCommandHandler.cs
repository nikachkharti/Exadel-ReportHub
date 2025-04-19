using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Plans.Commands;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Plans.Handlers.CommandHandlers
{
    public class UpdatePlanCommandHandler(IPlanRepository planRepository, IMapper mapper)
        : IRequestHandler<UpdatePlanCommand, string>
    {
        public async Task<string> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var existingPlan = await planRepository.Get(p => p.Id == request.Id, cancellationToken);

            if (existingPlan is null)
            {
                throw new NotFoundException($"Plan with id {request.Id} not found");
            }

            var updatedDocumentOfPlan = mapper.Map<Plan>(request);

            //TODO: [Optimization] we have to avoid double calling of database.
            await planRepository.UpdateSingleDocument(p => p.Id == request.Id, updatedDocumentOfPlan);
            return updatedDocumentOfPlan.Id;
        }
    }
}
