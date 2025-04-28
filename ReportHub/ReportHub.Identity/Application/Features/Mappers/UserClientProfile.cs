using AutoMapper;
using ReportHub.Identity.Application.Features.UserClients.DTOs;
using ReportHub.Identity.Domain.Entities;

namespace ReportHub.Identity.Application.Features.Mappers;

public class UserClientProfile : Profile
{
    public UserClientProfile()
    {
        CreateMap<UserClient, UserClientForGettingDto>()
            .ForMember(d => d.Id, s => s.MapFrom(u => u.Id))
            .ForMember(d => d.Role, s => s.MapFrom(u => u.Role))
            .ForMember(d => d.ClientId, s => s.MapFrom(u => u.ClientId));

        CreateMap<UserClient, ClientUserForGettingDto>()
            .ForMember(d => d.Id, s => s.MapFrom(u => u.Id))
            .ForMember(d => d.Role, s => s.MapFrom(u => u.Role))
            .ForMember(d => d.UserId, s => s.MapFrom(u => u.UserId));
    }
}
