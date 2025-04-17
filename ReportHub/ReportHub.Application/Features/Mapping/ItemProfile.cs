using AutoMapper;
using ReportHub.Application.Features.Item.DTOs;

namespace ReportHub.Application.Features.Mapping;

public class ItemProfile : Profile
{
    public ItemProfile()
    {
        CreateMap<Domain.Entities.Item, ItemForGettingDto>()
           .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
           .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
           .ForMember(dest => dest.Description, options => options.MapFrom(src => src.Description))
           .ForMember(dest => dest.Price, options => options.MapFrom(src => src.Price))
           .ForMember(dest => dest.Currency, options => options.MapFrom(src => src.Currency));
    }
}
