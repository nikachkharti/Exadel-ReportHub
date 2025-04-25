using AutoMapper;
using MediatR;
using ReportHub.Identity.Features.UserClients.DTOs;
using ReportHub.Identity.Features.UserClients.Queries;
using ReportHub.Identity.Repositories;
using ReportHub.Identity.Validators.Exceptions;

namespace ReportHub.Identity.Features.UserClients.Handlers.QueryHandlers;

public class GetUserClientsQueryHandler : IRequestHandler<GetUserClientsQuery, IEnumerable<UserClientForGettingDto>>
{
    private readonly IUserClientRepository _userClientRepository;
    private readonly IMapper _mapper;

    public GetUserClientsQueryHandler(IUserClientRepository userClientRepository, IMapper mapper)
    {
        _userClientRepository = userClientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserClientForGettingDto>> Handle(GetUserClientsQuery request, CancellationToken cancellationToken)
    {
        var userClients = await _userClientRepository.GetAllAsync(u => u.UserId.Equals(request.UserId));

        if(userClients is null || userClients.Any())
        {
            throw new NotFoundException($"There is no clients for user {request.UserId} yet");
        }

        return _mapper.Map<IEnumerable<UserClientForGettingDto>>(userClients);
    }
}
