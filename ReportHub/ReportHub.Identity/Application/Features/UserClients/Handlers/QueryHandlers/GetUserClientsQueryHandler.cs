using AutoMapper;
using MediatR;
using ReportHub.Identity.Application.Features.UserClients.DTOs;
using ReportHub.Identity.Application.Features.UserClients.Queries;
using ReportHub.Identity.Infrastructure.Repositories;

namespace ReportHub.Identity.Application.Features.UserClients.Handlers.QueryHandlers;

public class GetUserClientsQueryHandler : BaseGetAllQueryHandler, IRequestHandler<GetUserClientsQuery, IEnumerable<UserClientForGettingDto>>
{
    private readonly IMapper _mapper;

    public GetUserClientsQueryHandler(IUserClientRepository userClientRepository, IMapper mapper)
        : base(userClientRepository)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserClientForGettingDto>> Handle(GetUserClientsQuery request, CancellationToken cancellationToken)
    {
        var userClients = await GetAllAsync(u => u.UserId.Equals(request.UserId));

        return _mapper.Map<IEnumerable<UserClientForGettingDto>>(userClients);
    }
}
