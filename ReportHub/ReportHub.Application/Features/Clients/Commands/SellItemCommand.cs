using MediatR;

namespace ReportHub.Application.Features.Clients.Commands
{
    public record SellItemCommand
    (
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime SaleDate
    ) : IRequest<string>;
}
