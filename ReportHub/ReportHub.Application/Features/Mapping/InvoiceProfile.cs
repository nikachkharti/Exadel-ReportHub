using AutoMapper;
using ReportHub.Application.Features.DTOs;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Mappings;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<Invoice, InvoiceDto>();
    }
}
