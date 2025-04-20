using MediatR;

namespace ReportHub.Application.Features.Sale.Commands
{
    public record UpdateSaleCommand
    (
        string Id,
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime SaleDate
    ) : IRequest<string>;
}
