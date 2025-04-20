using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Commands;
using Serilog;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers
{
    public class SellItemCommandHandler(ISaleRepository saleRepository, IPlanRepository planRepository, IMapper mapper)
        : IRequestHandler<SellItemCommand, string>
    {
        public async Task<string> Handle(SellItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await saleRepository.Get(
                s => s.ClientId == request.ClientId && s.ItemId == request.ItemId,
                cancellationToken);

            string saleId;

            if (sale == null)
            {
                var newSale = mapper.Map<Domain.Entities.Sale>(request);
                newSale.SaleDate = DateTime.UtcNow;

                await saleRepository.Insert(newSale, cancellationToken);

                saleId = newSale.Id;

                Log.Information($"New sale {newSale.Id} for client {newSale.ClientId} inserted successfully");
            }
            else
            {
                var newAmount = sale.Amount + request.Amount;

                await saleRepository.UpdateMultipleFields
                (
                    s => s.ClientId == request.ClientId && s.ItemId == request.ItemId,
                    new Dictionary<Expression<Func<Domain.Entities.Sale, object>>, object>
                    {
                        { c => c.Amount, newAmount },
                        { c => c.SaleDate, DateTime.UtcNow }
                    },
                    cancellationToken
                );

                sale.Amount = newAmount; // update in-memory object for further checks
                saleId = sale.Id;

                Log.Information($"Existing sale {sale.Id} for client {sale.ClientId} updated successfully");
            }

            // After sale is processed — check if plan needs to be completed
            var plan = await planRepository.Get(
                p => p.ClientId == request.ClientId && p.ItemId == request.ItemId,
                cancellationToken);

            if (plan != null &&
                plan.Status == Domain.Entities.PlanStatus.InProgress &&
                plan.EndDate > DateTime.UtcNow &&
                sale.Amount >= plan.Amount)
            {
                await planRepository.UpdateSingleField
                (
                    p => p.ClientId == request.ClientId && p.ItemId == request.ItemId,
                    p => p.Status,
                    Domain.Entities.PlanStatus.Completed,
                    cancellationToken
                );

                Log.Information($"Plan for client {plan.ClientId} and item {plan.ItemId} marked as completed.");
            }
            else if (plan == null || plan.Status == Domain.Entities.PlanStatus.Canceled)
            {
                Log.Warning("Plan not found or is canceled.");
            }

            return saleId;
        }
    }
}
