using MediatR;
using Microsoft.AspNetCore.Identity;
using ReportHub.Identity.Features.UserClientRoles.Commands;
using ReportHub.Identity.Models;
using ReportHub.Identity.Repositories;
using ReportHub.Identity.Validators.Exceptions;

namespace ReportHub.Identity.Features.UserClientRoles.Handlers.CommandHandlers;

public class CreateUserClientCommandHandler : IRequestHandler<CreateUserClientRoleCommand, string>
{
    private readonly IUserClientRoleRepository _userClientRoleRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserManager<User> _userManeger;
    private readonly RoleManager<Role> _roleManeger;

    public CreateUserClientCommandHandler(
        RoleManager<Role> roleManeger, 
        UserManager<User> userManeger, 
        IUserClientRoleRepository userClientRoleRepository,
        IHttpClientFactory httpClientFactory)
    {
        _roleManeger = roleManeger;
        _userManeger = userManeger;
        _userClientRoleRepository = userClientRoleRepository;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> Handle(CreateUserClientRoleCommand request, CancellationToken cancellationToken)
    {
        var userExist = await _userManeger.FindByIdAsync(request.UserId) ?? 
                                            throw new NotFoundException(nameof(User), request.UserId);

        //var httpClient = _httpClientFactory.CreateClient();

        //var clientExist = httpClient.GetAsync("");

        var roleExist = await _roleManeger.FindByIdAsync(request.RoleId) ?? 
                                            throw new NotFoundException(nameof(Role), request.RoleId);

        var isInRole = await _userClientRoleRepository
                                        .GetAsync(r => r.UserId == request.UserId && request.ClientId == r.ClientId) is not null;

        if (isInRole)
        {
            throw new ConflictException($"User {request.UserId} has already in role for client {request.ClientId}");
        }
        
        if(roleExist.Name == "Owner" && 
            await _userClientRoleRepository.GetAsync(r => r.ClientId == request.ClientId && roleExist.Id == r.RoleId) is not null)
        {
            throw new ConflictException($"Client {request.ClientId} already has owner");
        }

        var userClient = new UserClientRole { UserId = request.UserId, RoleId = request.RoleId, ClientId = request.ClientId };

        await _userClientRoleRepository.InsertAsync(userClient);

        return userClient.Id;
    }
}
