﻿using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Plans.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Plans.Handlers.CommandHandlers
{
    public class DeletePlanCommandHandler(IPlanRepository planRepository, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<DeletePlanCommand, string>
    {
        public async Task<string> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var plan = await planRepository.Get(p => p.Id == request.Id, cancellationToken);

            //EnsureUserHasRoleForThisClient(plan.ClientId); TODO Uncomment when I add authorization on web

            if (plan is null)
            {
                throw new NotFoundException($"Plan with id {request.Id} not found");
            }

            //TODO: [Optimization] we have to avoid double calling of database.
            await planRepository.Delete(p => p.Id == request.Id, cancellationToken);
            return plan.Id;
        }
    }
}
