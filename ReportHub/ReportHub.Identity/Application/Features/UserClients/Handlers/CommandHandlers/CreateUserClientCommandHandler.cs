//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using ReportHub.Identity.Application.Features.UserClients.Commands;
//using ReportHub.Identity.Domain.Entities;
//using ReportHub.Identity.Infrastructure.Repositories;

//namespace ReportHub.Identity.Application.Features.UserClients.Handlers.CommandHandlers;

//public class CreateUserClientCommandHandler : IRequestHandler<CreateUserClientCommand, string>
//{
//    private readonly IUserClientRepository _userClientRoleRepository;
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly UserManager<User> _userManeger;

//    public CreateUserClientCommandHandler(
//        RoleManager<Role> roleManeger, 
//        UserManager<User> userManeger, 
//        IUserClientRepository userClientRoleRepository,
//        IHttpClientFactory httpClientFactory)
//    {
//        _roleManeger = roleManeger;
//        _userManeger = userManeger;
//        _userClientRoleRepository = userClientRoleRepository;
//        _httpClientFactory = httpClientFactory;
//    }

//    public async Task<string> Handle(CreateUserClientCommand request, CancellationToken cancellationToken)
//    {
//        var userExist = await _userManeger.FindByIdAsync(request.UserId) ?? 
//                                            throw new NotFoundException(nameof(User), request.UserId);

//        //var httpClient = _httpClientFactory.CreateClient();

//        //var clientExist = httpClient.GetAsync("");

//        var roleExist = await _roleManeger.FindByIdAsync(request.Role) ?? 
//                                            throw new NotFoundException(nameof(Role), request.Role);

//        var isInRole = await _userClientRoleRepository
//                                        .GetAsync(r => r.UserId == request.UserId && request.ClientId == r.ClientId) is not null;

//        if (isInRole)
//            throw new ConflictException($"User {request.UserId} has already in role for client {request.ClientId}");
        
//        if(roleExist.Name == "Owner" && 
//            await _userClientRoleRepository.GetAsync(r => r.ClientId == request.ClientId && roleExist.Id == r.Role) is not null)
//        {
//            throw new ConflictException($"Client {request.ClientId} already has owner");
//        }

//        var userClient = new UserClient { UserId = request.UserId, Role = request.Role, ClientId = request.ClientId };

//        await _userClientRoleRepository.InsertAsync(userClient);

//        return userClient.Id;
//    }
//}
