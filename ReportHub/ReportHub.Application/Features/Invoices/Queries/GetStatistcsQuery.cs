using MediatR;
using ReportHub.Application.Features.Invoices.DTOs;

namespace ReportHub.Application.Features.Invoices.Queries;

public record GetStatistcsQuery(string PaymentStatus, string Currency) : IRequest<StatisticsDto>;
