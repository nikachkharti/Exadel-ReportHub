using AutoMapper;
using ReportHub.Identity.Features.UserClientRoles.DTOs;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Features.Mappers;

public class UserClientProfile : Profile
{
    public UserClientProfile()
    {
        CreateMap<UserClientRole, MyClientForGettingDto>()
            .ForMember(d => d.RoleId, s => s.MapFrom(u => u.RoleId))
            .ForMember(d => d.ClientId, s => s.MapFrom(u => u.ClientId));
    }
}
