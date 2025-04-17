using AutoMapper;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Features.Customers.DTOs;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Item.DTOs;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Mapping;

public class ReportHubMappingProfile : Profile
{
    public ReportHubMappingProfile()
    {
        #region CUSTOMER

        CreateMap<Customer, CustomerForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, options => options.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, options => options.MapFrom(src => src.Address));

        CreateMap<CreateCustomerCommand, Customer>()
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, options => options.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, options => options.MapFrom(src => src.Address));


        CreateMap<UpdateCustomerCommand, Customer>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, options => options.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, options => options.MapFrom(src => src.Address));

        #endregion



        #region ITEM

        CreateMap<Domain.Entities.Item, ItemForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, options => options.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, options => options.MapFrom(src => src.Price))
            .ForMember(dest => dest.Currency, options => options.MapFrom(src => src.Currency));

        #endregion

    }
}
