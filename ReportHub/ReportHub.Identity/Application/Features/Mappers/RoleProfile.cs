using AutoMapper;
using ReportHub.Identity.Application.Features.Roles.DTOs;
using ReportHub.Identity.Domain.Entities;

namespace ReportHub.Identity.Application.Features.Mappers;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleForGettingDto>()
            .ForMember(d => d.Id, s => s.MapFrom(r => r.Id))
            .ForMember(d => d.Name, s => s.MapFrom(r => r.Name));
    }
}
