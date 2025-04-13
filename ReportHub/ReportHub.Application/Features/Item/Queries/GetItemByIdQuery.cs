using MediatR;
using ReportHub.Application.Features.Item.DTOs;

namespace ReportHub.Application.Features.Item.Queries
{
    public record GetItemByIdQuery(string Id) : IRequest<ItemForGettingDto>;
}
