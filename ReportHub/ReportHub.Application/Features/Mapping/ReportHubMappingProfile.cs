using AutoMapper;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Mapping;

public class ReportHubMappingProfile : Profile
{
    public ReportHubMappingProfile()
    {
        CreateMap<Invoice, InvoiceForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.CustomerId, options => options.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.IssueDate, options => options.MapFrom(src => src.IssueDate))
            .ForMember(dest => dest.DueDate, options => options.MapFrom(src => src.DueDate))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Currency, options => options.MapFrom(src => src.Currency))
            .ForMember(dest => dest.PaymentStatus, options => options.MapFrom(src => src.PaymentStatus))
            .ForMember(dest => dest.ItemIds, options => options.MapFrom(src => src.ItemIds));

        CreateMap<Client, ClientForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Specialization, options => options.MapFrom(src => src.Specialization));

        CreateMap<ClientForCreatingDto, Client>()
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Specialization, options => options.MapFrom(src => src.Specialization));

        CreateMap<ClientForUpdatingDto, Client>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Specialization, options => options.MapFrom(src => src.Specialization));
    }
}
