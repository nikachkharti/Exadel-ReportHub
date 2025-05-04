using MediatR;
using ReportHub.Application.Features.Clients.DTOs;

namespace ReportHub.Application.Features.Clients.Queries;

public record GetRevenueInRangeQuery(DateTime FromDate, DateTime ToDate, string CurrencyCode) : IRequest<RevenueDto>;
