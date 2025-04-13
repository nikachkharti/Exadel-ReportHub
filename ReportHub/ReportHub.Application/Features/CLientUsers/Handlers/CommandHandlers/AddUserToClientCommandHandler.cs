using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Handlers.CommandHandlers
{
    public class AddUserToClientCommandHandler : IRequestHandler<AddUserToClientCommand, bool>
    {
        private readonly IClientUserRepository _clientUserRepository;
        private readonly IIdentityService _identityService;

        public AddUserToClientCommandHandler
            (IClientUserRepository clientUserRepository,
            IIdentityService identityService)
        {
            _clientUserRepository = clientUserRepository;
            _identityService = identityService;
        }

        public async Task<bool> Handle(AddUserToClientCommand request, CancellationToken cancellationToken)
        {
            var clientUser = new ClientUser
            {
                ClientId = request.ClientId,
                UserId = request.UserId,
                Role = request.Role
            };

            var roleAssigned = await _identityService.AssignUserRole(request.UserId, request.Role);

            if (!roleAssigned)
            {
                return false;
            }
            
            await _clientUserRepository.Insert(clientUser, cancellationToken);

            return true;
        }
    }
}
