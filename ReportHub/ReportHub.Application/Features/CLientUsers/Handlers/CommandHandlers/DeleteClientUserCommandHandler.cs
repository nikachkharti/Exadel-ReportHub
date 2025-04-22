using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Domain.Entities;
using System.Linq.Expressions;

namespace ReportHub.Application.Features.CLientUsers.Handlers.CommandHandlers;

public class DeleteClientUserCommandHandler : IRequestHandler<DeleteClientUserCommand, string>
{
    private readonly IClientUserRepository _clientUserRepository;

    public DeleteClientUserCommandHandler(IClientUserRepository clientUserRepository)
    {
        _clientUserRepository = clientUserRepository;
    }

    public async Task<string> Handle(DeleteClientUserCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<ClientUser, bool>> expression = (ClientUser r) => r.UserId == request.UserId && r.ClientId == request.ClientId;
        
        var clientUser = await EnsureEntityExist(expression, cancellationToken);

        await _clientUserRepository.Delete(expression);

        return clientUser.Id;
    }

    private async Task<ClientUser> EnsureEntityExist(Expression<Func<ClientUser, bool>> expression, CancellationToken cancellationToken)
    {
        return await _clientUserRepository.Get(expression) ?? 
                            throw new NotFoundException("Client role does not exist");
    }
}
