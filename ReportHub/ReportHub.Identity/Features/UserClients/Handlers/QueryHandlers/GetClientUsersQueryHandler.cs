using AutoMapper;
using MediatR;
using ReportHub.Identity.Features.UserClients.DTOs;
using ReportHub.Identity.Features.UserClients.Queries;
using ReportHub.Identity.Repositories;
using ReportHub.Identity.Validators.Exceptions;

namespace ReportHub.Identity.Features.UserClients.Handlers.QueryHandlers;

public class GetClientUsersQueryHandler : IRequestHandler<GetClientUsersQuery, IEnumerable<ClientUserForGettingDto>>
{
    private readonly IUserClientRepository _userClientRepository;
    private readonly IMapper _mapper;

    public GetClientUsersQueryHandler(IUserClientRepository userClientRepository, IMapper mapper)
    {
        _userClientRepository = userClientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClientUserForGettingDto>> Handle(GetClientUsersQuery request, CancellationToken cancellationToken)
    {
        var clientUsers = await _userClientRepository.GetAllAsync(u => request.Equals(u.ClientId));

        if (clientUsers is null || clientUsers.Any())
        {
            throw new NoContentException($"There is no users for client {request.clientId} yet");
        }

        return _mapper.Map<IEnumerable<ClientUserForGettingDto>>(clientUsers);

    }
}
