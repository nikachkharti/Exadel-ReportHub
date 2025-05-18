using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Customers.Handlers.CommandHandlers
{
    public class CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper, IRequestContextService requestContext)
        : BaseFeature(requestContext), IRequestHandler<CreateCustomerCommand, string>
    {
        public async Task<string> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            //EnsureUserHasRoleForThisClient(request.ClientId);

            var customer = mapper.Map<Customer>(request);

            await customerRepository.Insert(customer);
            return customer.Id;
        }
    }
}
