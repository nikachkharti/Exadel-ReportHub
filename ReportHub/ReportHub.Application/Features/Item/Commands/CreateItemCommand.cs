using MediatR;

namespace ReportHub.Application.Features.Item.Commands
{
    public record CreateItemCommand(string ClientId, string Name, string Description, decimal Price, string Currency) : IRequest<string>;
}
