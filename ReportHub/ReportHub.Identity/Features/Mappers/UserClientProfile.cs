using AutoMapper;
using ReportHub.Identity.Features.UserClients.DTOs;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Features.Mappers;

public class UserClientProfile : Profile
{
    public UserClientProfile()
    {
        CreateMap<UserClient, UserClientForGettingDto>()
            .ForMember(d => d.Role, s => s.MapFrom(u => u.Role))
            .ForMember(d => d.ClientId, s => s.MapFrom(u => u.ClientId ?? "System"));

        CreateMap<UserClient, ClientUserForGettingDto>()
            .ForMember(d => d.Role, s => s.MapFrom(u => u.Role))
            .ForMember(d => d.UserId, s => s.MapFrom(u => u.UserId));
    }
}
