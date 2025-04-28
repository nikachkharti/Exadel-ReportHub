using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Customers.Handlers.CommandHandlers
{
    public class DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<DeleteCustomerCommand, string>
    {
        public async Task<string> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var customer = await customerRepository.Get(c => c.Id == request.Id, cancellationToken);

            if (customer is null)
            {
                throw new NotFoundException($"Customer with id {request.Id} not found");
            }

            EnsureUserHasRoleForThisClient(customer.ClientId);
            //TODO: [Optimization] we have to avoid double calling of database.
            await customerRepository.Delete(c => c.Id == request.Id, cancellationToken);
            return customer.Id;
        }
    }
}
