using MediatR;

namespace ReportHub.Application.Features.Item.Commands
{
    public record DeleteItemCommand(string Id) : IRequest<string>;
}
