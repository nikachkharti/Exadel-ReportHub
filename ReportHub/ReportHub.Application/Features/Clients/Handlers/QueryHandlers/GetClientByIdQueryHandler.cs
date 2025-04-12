using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Clients.Queries;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.Clients.Handlers.QueryHandlers
{
    public class GetClientByIdQueryHandler(IClientRepository clientRepository, IMapper mapper)
        : IRequestHandler<GetClientByIdQuery, ClientForGettingDto>
    {
        public async Task<ClientForGettingDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            var client = await clientRepository.Get(c => c.Id == request.Id, cancellationToken);

            if (client is null)
            {
                throw new NotFoundException($"Client with id {request.Id} not found");
            }

            return mapper.Map<ClientForGettingDto>(client);
        }
    }
}
