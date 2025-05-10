using AutoMapper;
using ReportHub.Application.Features.Clients.Commands;
using ReportHub.Application.Features.Clients.DTOs;
using ReportHub.Application.Features.Customers.Commands;
using ReportHub.Application.Features.Customers.DTOs;
using ReportHub.Application.Features.InvoiceLogs.Commands;
using ReportHub.Application.Features.InvoiceLogs.DTOs;
using ReportHub.Application.Features.Invoices.DTOs;
using ReportHub.Application.Features.Items.Commands;
using ReportHub.Application.Features.Items.DTOs;
using ReportHub.Application.Features.Plans.Commands;
using ReportHub.Application.Features.Plans.DTOs;
using ReportHub.Application.Features.ReportSchedule.Commands;
using ReportHub.Application.Features.ReportSchedule.DTOs;
using ReportHub.Application.Features.Sale.Commands;
using ReportHub.Application.Features.Sale.DTOs;
using ReportHub.Domain.Entities;

namespace ReportHub.Application.Features.Mapping;

public class ReportHubMappingProfile : Profile
{
    public ReportHubMappingProfile()
    {
        #region INVOICE
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
        #endregion

        #region CLIENT
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
        #endregion


        #region CUSTOMER

        CreateMap<Customer, CustomerForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, options => options.MapFrom(src => src.Email))
            .ForMember(dest => dest.CountryId, options => options.MapFrom(src => src.CountryId));

        CreateMap<CreateCustomerCommand, Customer>()
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, options => options.MapFrom(src => src.Email))
            .ForMember(dest => dest.CountryId, options => options.MapFrom(src => src.CountryId));


        CreateMap<UpdateCustomerCommand, Customer>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, options => options.MapFrom(src => src.Email))
            .ForMember(dest => dest.CountryId, options => options.MapFrom(src => src.CountryId));

        #endregion



        #region ITEM

        CreateMap<Item, ItemForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, options => options.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, options => options.MapFrom(src => src.Price))
            .ForMember(dest => dest.Currency, options => options.MapFrom(src => src.Currency));

        CreateMap<CreateItemCommand, Item>()
            .ForMember(dest => dest.Name, options => options.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, options => options.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, options => options.MapFrom(src => src.Price))
            .ForMember(dest => dest.Currency, options => options.MapFrom(src => src.Currency));


        #endregion


        #region PLAN

        CreateMap<Plan, PlanForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.ItemId, options => options.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.StartDate, options => options.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, options => options.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Status, options => options.MapFrom(src => src.Status))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount));

        CreateMap<CreatePlanCommand, Plan>()
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.ItemId, options => options.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.StartDate, options => options.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, options => options.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Status, options => options.MapFrom(src => src.Status))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount));


        CreateMap<UpdatePlanCommand, Plan>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.ItemId, options => options.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.StartDate, options => options.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate, options => options.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Status, options => options.MapFrom(src => src.Status))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount));


        CreateMap<UpdatePlanStatusCommand, Plan>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, options => options.MapFrom(src => src.Status));

        #endregion


        #region SALE

        CreateMap<Domain.Entities.Sale, SaleForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.ItemId, options => options.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount))
            .ForMember(dest => dest.SaleDate, options => options.MapFrom(src => src.SaleDate));

        CreateMap<CreateSaleCommand, Domain.Entities.Sale>()
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.ItemId, options => options.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount))
            .ForMember(dest => dest.SaleDate, options => options.MapFrom(src => src.SaleDate));

        CreateMap<SellItemCommand, Domain.Entities.Sale>()
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.ItemId, options => options.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount))
            .ForMember(dest => dest.SaleDate, options => options.MapFrom(src => src.SaleDate));

        CreateMap<UpdateSaleCommand, Domain.Entities.Sale>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClientId, options => options.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.ItemId, options => options.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.Amount, options => options.MapFrom(src => src.Amount))
            .ForMember(dest => dest.SaleDate, options => options.MapFrom(src => src.SaleDate));

        #endregion

        #region INVOICELOG

        CreateMap<InvoiceLog, InvoiceLogForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, options => options.MapFrom(src => src.UserId))
            .ForMember(dest => dest.InvoiceId, options => options.MapFrom(src => src.InvoiceId))
            .ForMember(dest => dest.TimeStamp, options => options.MapFrom(src => src.TimeStamp))
            .ForMember(dest => dest.Status, options => options.MapFrom(src => src.Status));

        CreateMap<CreateInvoiceLogCommand, InvoiceLog>()
            .ForMember(dest => dest.UserId, options => options.MapFrom(src => src.UserId))
            .ForMember(dest => dest.InvoiceId, options => options.MapFrom(src => src.InvoiceId))
            .ForMember(dest => dest.TimeStamp, options => options.MapFrom(src => src.TimeStamp))
            .ForMember(dest => dest.Status, options => options.MapFrom(src => src.Status));

        #endregion


        #region REPORTSCHEDULE

        CreateMap<Domain.Entities.ReportSchedule, ReportScheduleForGettingDto>()
            .ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id))
            .ForMember(dest => dest.CustomerId, options => options.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.CronExpression, options => options.MapFrom(src => src.CronExpression))
            .ForMember(dest => dest.Format, options => options.MapFrom(src => src.Format))
            .ForMember(dest => dest.IsActive, options => options.MapFrom(src => src.IsActive));

        CreateMap<CreateReportScheduleCommand, Domain.Entities.ReportSchedule>()
            .ForMember(dest => dest.CustomerId, options => options.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.CronExpression, options => options.MapFrom(src => src.CronExpression))
            .ForMember(dest => dest.Format, options => options.MapFrom(src => src.Format))
            .ForMember(dest => dest.IsActive, options => options.MapFrom(src => src.IsActive));

        CreateMap<UpdateReportScheduleCommand, Domain.Entities.ReportSchedule>()
            .ForMember(dest => dest.CustomerId, options => options.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.CronExpression, options => options.MapFrom(src => src.CronExpression))
            .ForMember(dest => dest.Format, options => options.MapFrom(src => src.Format))
            .ForMember(dest => dest.IsActive, options => options.MapFrom(src => src.IsActive));

        #endregion
    }
}
