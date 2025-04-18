﻿using MediatR;
using ReportHub.Application.Features.Plans.DTOs;

namespace ReportHub.Application.Features.Plans.Queries
{
    public record GetPlansOfCustomer
    (
        int? PageNumber,
        int? PageSize,
        string SortingParameter,
        bool Ascending = false,
        CancellationToken CancellationToken = default
    ) : IRequest<IEnumerable<PlanForGettingDto>>;
}
