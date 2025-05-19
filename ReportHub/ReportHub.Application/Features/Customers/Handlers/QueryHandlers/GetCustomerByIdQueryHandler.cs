using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Customers.DTOs;
using ReportHub.Application.Features.Customers.Queries;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Customers.Handlers.QueryHandlers
{
    public class GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper, IRequestContextService requestContext)
        : IRequestHandler<GetCustomerByIdQuery, CustomerForGettingDto>
    {
        public async Task<CustomerForGettingDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var clientId = requestContext.GetClientId();

            var customer = await customerRepository.Get(c => 
                                        c.ClientId == clientId && c.Id == request.Id, cancellationToken);

            if (customer is null)
            {
                throw new NotFoundException($"Customer with id {request.Id} not found");
            }

            //EnsureUserHasRoleForThisClient(customer.ClientId);

            return mapper.Map<CustomerForGettingDto>(customer);
        }
    }
}
