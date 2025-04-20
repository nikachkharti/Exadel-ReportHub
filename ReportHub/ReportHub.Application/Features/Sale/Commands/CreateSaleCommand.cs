using MediatR;

namespace ReportHub.Application.Features.Sale.Commands
{
    public record CreateSaleCommand
    (
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime SaleDate
    ) : IRequest<string>;
}
