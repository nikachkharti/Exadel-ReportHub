using AutoMapper;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Invoices.Mapping;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<Invoice, InvoiceDto>();
    }
}
