using MediatR;

namespace ReportHub.Application.Features.Item.Commands
{
    public record DeleteItemOfClientCommand(string ClientId, string ItemId) : IRequest<string>;

}
