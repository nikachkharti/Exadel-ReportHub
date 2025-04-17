using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Handlers.CommandHandlers
{
    public class AddUserToClientCommandHandler : IRequestHandler<AddUserToClientCommand, bool>
    {
        private readonly IClientUserRepository _clientUserRepository;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public AddUserToClientCommandHandler
            (IClientUserRepository clientUserRepository,
            IIdentityService identityService,
            IMediator mediator)
        {
            _clientUserRepository = clientUserRepository;
            _identityService = identityService;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AddUserToClientCommand request, CancellationToken cancellationToken)
        {
            await EnsureEntitiesExist(request.UserId, request.ClientId,cancellationToken);

            var clientUser = new ClientUser
            {
                ClientId = request.ClientId,
                UserId = request.UserId,
                Role = request.Role
            };

            await Task.WhenAll(
                _clientUserRepository.Insert(clientUser, cancellationToken),
                MakeUserAdminIfRoleAppropraite(request.Role, request.UserId, cancellationToken));

            return true;
        }

        private async Task MakeUserAdminIfRoleAppropraite(string role, string userId, CancellationToken cancellationToken)
        {
            string[] roles = ["SuperAdmin", "Owner", "ClientAdmin"];

            if (roles.Any(r => r.Equals(role)))
            {
                await _identityService.AssignUserRole(userId, cancellationToken);
            }
        }

        private async Task EnsureEntitiesExist(string userId, string clientId, CancellationToken cancellationToken)
        {
             await _mediator.Send(new GetClientByIdQuery(clientId), cancellationToken);
            await _identityService.ValidateUserIdExists(userId, cancellationToken);
        }
    }
}
