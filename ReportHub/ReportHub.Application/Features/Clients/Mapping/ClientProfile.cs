using AutoMapper;
using ReportHub.Application.Contracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Clients.Mapping;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<Client, ClientDto>();
    }
}
