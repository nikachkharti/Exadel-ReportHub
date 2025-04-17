using AutoMapper;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Mapping;

public class ClientUserProfile : Profile
{
    public ClientUserProfile()
    {
        CreateMap<ClientUser, AddUserToClientCommand>().ReverseMap();
    }
}
