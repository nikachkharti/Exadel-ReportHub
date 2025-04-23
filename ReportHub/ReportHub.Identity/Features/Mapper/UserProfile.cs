using AutoMapper;
using ReportHub.Identity.Features.Users.Commands;
using ReportHub.Identity.Models;

namespace ReportHub.Identity.Features.Mapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, User>();
    }
}
