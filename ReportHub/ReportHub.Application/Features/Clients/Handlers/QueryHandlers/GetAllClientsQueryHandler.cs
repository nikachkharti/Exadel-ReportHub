using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Clients.Queries;

namespace ReportHub.Application.Features.Clients.Handlers.QueryHandlers
{
    public class GetAllClientsQueryHandler(IClientRepository clientRepository, IMapper mapper)
        : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientForGettingDto>>
    {
        public async Task<IEnumerable<ClientForGettingDto>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
        {
            var clients = await clientRepository.GetAll
            (
                request.PageNumber ?? 1,
                request.PageSize ?? 10,
                sortBy: c => request.SortingParameter,
                ascending: request.Ascending,
                cancellationToken
            );

            if (clients.Any())
            {
                return mapper.Map<IEnumerable<ClientForGettingDto>>(clients);
            }

            return Enumerable.Empty<ClientForGettingDto>();
        }
    }
}
