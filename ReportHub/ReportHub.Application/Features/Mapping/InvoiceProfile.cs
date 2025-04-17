using AutoMapper;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Mapping;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
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
    }
}
