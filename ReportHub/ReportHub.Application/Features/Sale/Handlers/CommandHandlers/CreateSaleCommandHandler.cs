using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Sale.Commands;

namespace ReportHub.Application.Features.Sale.Handlers.CommandHandlers
{
    public class CreateSaleCommandHandler(ISaleRepository saleRepository, IMapper mapper) :
        IRequestHandler<CreateSaleCommand, string>
    {
        public async Task<string> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            //TODO: [Add] validators.
            var sale = mapper.Map<Domain.Entities.Sale>(request);

            await saleRepository.Insert(sale, cancellationToken);
            return sale.Id;
        }
    }
}
