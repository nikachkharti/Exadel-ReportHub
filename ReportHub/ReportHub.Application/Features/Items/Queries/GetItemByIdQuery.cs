using MediatR;
using ReportHub.Application.Features.Items.DTOs;

namespace ReportHub.Application.Features.Items.Queries
{
    public record GetItemByIdQuery(string Id) : IRequest<ItemForGettingDto>;
}
