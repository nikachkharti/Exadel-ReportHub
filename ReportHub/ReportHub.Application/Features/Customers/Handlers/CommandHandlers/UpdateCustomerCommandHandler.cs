using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Customers.Handlers.CommandHandlers
{
    public class UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
        : IRequestHandler<UpdateCustomerCommand, string>
    {
        public async Task<string> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var existingCustomer = await customerRepository.Get(c => c.Id == request.Id, cancellationToken);

            if (existingCustomer is null)
            {
                throw new NotFoundException($"Customer with id {request.Id} not found");
            }

            var updatedDocumentOfCustomer = mapper.Map<Customer>(request);

            //TODO: [Optimization] we have to avoid double calling of database.
            await customerRepository.UpdateSingleDocument(c => c.Id == request.Id, updatedDocumentOfCustomer);
            return updatedDocumentOfCustomer.Id;
        }
    }
}
