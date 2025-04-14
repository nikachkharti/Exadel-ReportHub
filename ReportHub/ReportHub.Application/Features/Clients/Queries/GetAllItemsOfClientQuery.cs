using MediatR;
using ReportHub.Application.Features.Item.DTOs;

namespace ReportHub.Application.Features.Clients.Queries
{
    public record GetAllItemsOfClientQuery(string ClientId) : IRequest<IEnumerable<ItemForGettingDto>>;
}
