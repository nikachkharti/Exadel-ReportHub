using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Sale.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Sale.Handlers.CommandHandlers
{
    public class DeleteSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper)
        : IRequestHandler<DeleteSaleCommand, string>
    {
        public async Task<string> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var sale = await saleRepository.Get(s => s.Id == request.Id, cancellationToken);

            if (sale is null)
            {
                throw new NotFoundException($"Sale with id {request.Id} not found");
            }

            //TODO: [Optimization] we have to avoid double calling of database.
            await saleRepository.Delete(c => c.Id == request.Id, cancellationToken);
            return sale.Id;
        }
    }
}
