using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Commands;
using Serilog;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers
{
    public class SellItemCommandHandler(ISaleRepository saleRepository, IMapper mapper)
        : IRequestHandler<SellItemCommand, string>
    {
        public async Task<string> Handle(SellItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await saleRepository.Get(s => s.ClientId == request.ClientId && s.ItemId == request.ItemId);

            if (sale is null)
            {
                var newSale = mapper.Map<Domain.Entities.Sale>(request);
                newSale.SaleDate = DateTime.Now;

                await saleRepository.Insert(newSale);

                Log.Information("New sale inserted successfully");
                return newSale.Id;
            }
            else
            {
                var newAmount = sale.Amount += request.Amount;

                await saleRepository.UpdateMultipleFields
                (
                    s => s.ClientId == request.ClientId && s.ItemId == request.ItemId,
                    new Dictionary<Expression<Func<Domain.Entities.Sale, object>>, object>()
                    {
                        { c => c.Amount, newAmount},
                        { c => c.SaleDate, DateTime.Now }
                    }
                );

                Log.Information("Existed sale updated successfully");
                return sale.Id;
            }

        }
    }
}
