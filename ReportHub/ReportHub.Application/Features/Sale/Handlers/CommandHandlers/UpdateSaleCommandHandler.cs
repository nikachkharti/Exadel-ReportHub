using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Sale.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Sale.Handlers.CommandHandlers
{
    public class UpdateSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper)
        : IRequestHandler<UpdateSaleCommand, string>
    {
        public async Task<string> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.

            var existingSale = await saleRepository.Get(c => c.Id == request.Id, cancellationToken);

            if (existingSale is null)
            {
                throw new NotFoundException($"Sale with id {request.Id} not found");
            }

            var updatedDocumentOfSale = mapper.Map<Domain.Entities.Sale>(request);

            //TODO: [Optimization] we have to avoid double calling of database.
            await saleRepository.UpdateSingleDocument(s => s.Id == request.Id, updatedDocumentOfSale);
            return updatedDocumentOfSale.Id;
        }
    }
}
