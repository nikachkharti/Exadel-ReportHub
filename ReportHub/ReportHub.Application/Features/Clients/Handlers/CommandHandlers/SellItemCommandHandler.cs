using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Commands;
using Serilog;

namespace ReportHub.Application.Features.Clients.Handlers.CommandHandlers
{
    public class SellItemCommandHandler(ISaleRepository saleRepository, IPlanRepository planRepository, IMapper mapper)
        : IRequestHandler<SellItemCommand, string>
    {
        public async Task<string> Handle(SellItemCommand request, CancellationToken cancellationToken)
        {
            // Always insert a new sale
            var newSale = mapper.Map<Domain.Entities.Sale>(request);
            newSale.SaleDate = DateTime.UtcNow;

            await saleRepository.Insert(newSale, cancellationToken);

            var saleId = newSale.Id;

            Log.Information($"New sale {newSale.Id} for client {newSale.ClientId} inserted successfully");

            // Calculate total sales for this client and item
            var totalAmount = (await saleRepository.GetAll(
                s => s.ClientId == request.ClientId && s.ItemId == request.ItemId,
                cancellationToken))
                .Sum(s => s.Amount);

            // Check and update plan status if needed
            var plan = await planRepository.Get(
                p => p.ClientId == request.ClientId &&
                p.ItemId == request.ItemId &&
                p.IsDeleted == false,
                cancellationToken);

            if (plan != null &&
                plan.Status == Domain.Entities.PlanStatus.InProgress &&
                plan.EndDate > DateTime.UtcNow &&
                plan.IsDeleted == false &&
                totalAmount >= plan.Amount)
            {
                await planRepository.UpdateSingleField(
                    p => p.ClientId == request.ClientId && p.ItemId == request.ItemId,
                    p => p.Status,
                    Domain.Entities.PlanStatus.Completed,
                    cancellationToken);

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
