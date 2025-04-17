using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ClientRoles.Queries;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.CLientUsers.Handlers.CommandHandlers
{
    public class AddUserToClientCommandHandler : IRequestHandler<AddUserToClientCommand, bool>
    {
        private readonly IClientUserRepository _clientUserRepository;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AddUserToClientCommandHandler
            (IClientUserRepository clientUserRepository,
            IIdentityService identityService,
            IMediator mediator,
            IMapper mapper)
        {
            _clientUserRepository = clientUserRepository;
            _identityService = identityService;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<bool> Handle(AddUserToClientCommand request, CancellationToken cancellationToken)
        {
            await EnsureEntitiesExist(request,cancellationToken);
            await EnsureEntityDoesNotExist(request.UserId, request.ClientId, cancellationToken);

            var clientUser = _mapper.Map<ClientUser>(request);

            await _clientUserRepository.Insert(clientUser, cancellationToken);

            await MakeUserAdminIfRoleAppropraite(request.Role, request.UserId, cancellationToken);

            return true;
        }

        /// <summary>
        /// Ensure that user does not exist for given client
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        private async Task EnsureEntityDoesNotExist(string userId, string clientId, CancellationToken cancellationToken)
        {
            var existingClientUser = await _clientUserRepository
                .Get(c => c.UserId.Equals(userId) && c.ClientId.Equals(clientId), cancellationToken);

            if (existingClientUser is not null)
            {
                throw new BadRequestException($"User with {userId} already exist in client {clientId}");
            }
        }

        /// <summary>
        /// Ensure that the client, role and user exist
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task EnsureEntitiesExist(AddUserToClientCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new GetClientByIdQuery(request.ClientId), cancellationToken);
            await _mediator.Send(new GetClientRoleByNameQuery(request.Role));
            await _identityService.ValidateUserIdExists(request.UserId, cancellationToken);
        }

        /// <summary>
        /// Make user admin if the role is appropriate
        /// Because these ["SuperAdmin", "Owner", "ClientAdmin"] roles should create User in identity side
        /// </summary>
        /// <param name="role"></param>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task MakeUserAdminIfRoleAppropraite(string role, string userId, CancellationToken cancellationToken)
        {
            string[] roles = ["SuperAdmin", "Owner", "ClientAdmin"];

            if (roles.Any(r => r.Equals(role)))
            {
                await _identityService.AssignUserRole(userId, cancellationToken);
            }
        }
    }
}
