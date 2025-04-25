using AutoMapper;
using MediatR;
using ReportHub.Identity.Application.Features.UserClients.DTOs;
using ReportHub.Identity.Application.Features.UserClients.Queries;
using ReportHub.Identity.Infrastructure.Repositories;

namespace ReportHub.Identity.Application.Features.UserClients.Handlers.QueryHandlers;

public class GetClientUsersQueryHandler : BaseGetAllQueryHandler, IRequestHandler<GetClientUsersQuery, IEnumerable<ClientUserForGettingDto>>
{
    private readonly IMapper _mapper;

    public GetClientUsersQueryHandler(IUserClientRepository userClientRepository, IMapper mapper)
        : base(userClientRepository)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClientUserForGettingDto>> Handle(GetClientUsersQuery request, CancellationToken cancellationToken)
    {
        var clientUsers = await GetAllAsync(u => request.Equals(u.ClientId));

        return _mapper.Map<IEnumerable<ClientUserForGettingDto>>(clientUsers);

    }
}
