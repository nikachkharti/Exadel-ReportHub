using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Handlers.CommandHandlers
{
    public class AddToUserCommandHandler : IRequestHandler<AddUserToClientCommand, bool>
    {
        private readonly IClientUserRepository _clientUserRepository;

        public AddToUserCommandHandler(IClientUserRepository clientUserRepository)
        {
            _clientUserRepository = clientUserRepository;
        }

        public async Task<bool> Handle(AddUserToClientCommand request, CancellationToken cancellationToken)
        {
            var clientUser = new ClientUser
            {
                ClientId = request.ClientId,
                UserId = request.UserId,
                Role = request.Role
            };

            await _clientUserRepository.Insert(clientUser, cancellationToken);

            return true;
        }
    }
}
