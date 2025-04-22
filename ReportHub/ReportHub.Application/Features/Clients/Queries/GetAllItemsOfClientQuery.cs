using MediatR;
using ReportHub.Application.Features.Items.DTOs;

namespace ReportHub.Application.Features.Clients.Queries
{
    public record GetAllItemsOfClientQuery(string ClientId) : IRequest<IEnumerable<ItemForGettingDto>>;
}
