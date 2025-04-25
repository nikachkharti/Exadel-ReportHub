using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver.Linq;
using ReportHub.Identity.Application.Features.Users.DTOs;
using ReportHub.Identity.Application.Features.Users.Queries;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;

namespace ReportHub.Identity.Application.Features.Users.Handlers.QueryHandlers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserForGettingDto>>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    public GetAllUsersQueryHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserForGettingDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await GetUsersAsList(request);
    }

    private async Task<IEnumerable<UserForGettingDto>> GetUsersAsList(GetAllUsersQuery request)
    {
        var users = await _userManager
              .Users
              .Skip(10 * (request.currentPage - 1))
              .Take(10)
              .ToListAsync();
              
         if(users is null || !users.Any()) throw new NoContentException();

        return _mapper.Map<IEnumerable<UserForGettingDto>>(users);
    }
}
