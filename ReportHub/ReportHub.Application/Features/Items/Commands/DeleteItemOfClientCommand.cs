using MediatR;

namespace ReportHub.Application.Features.Items.Commands
{
    public record DeleteItemOfClientCommand(string ClientId, string ItemId) : IRequest<string>;

}
