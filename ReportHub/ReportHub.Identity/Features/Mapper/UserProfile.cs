using AutoMapper;
using ReportHub.Identity.Features.Users.Commands;
using ReportHub.Identity.Features.Users.DTOs;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Features.Mapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.Email, src => src.MapFrom(c => c.Email))
            .ForMember(dest => dest.UserName, src => src.MapFrom(c => c.Username));

        CreateMap<User, UserForGettingDto>()
            .ForMember(dest => dest.Id, src => src.MapFrom(u => u.Id))
            .ForMember(dest => dest.Username, src => src.MapFrom(u => u.UserName))
            .ForMember(dest => dest.Email, src => src.MapFrom(u => u.Email));

    }
}
