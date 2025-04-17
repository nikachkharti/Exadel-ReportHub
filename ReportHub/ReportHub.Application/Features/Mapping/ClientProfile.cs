using AutoMapper;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Mapping;

public class ClientProfile : Profile
{
    public ClientProfile()
    {
        CreateMap<Client, ClientForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Specialization, options => options.MapFrom(src => src.Specialization));

        CreateMap<CreateClientCommand, Client>()
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Specialization, options => options.MapFrom(src => src.Specialization));

        CreateMap<UpdateClientCommand, Client>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Specialization, options => options.MapFrom(src => src.Specialization));
    }
}
