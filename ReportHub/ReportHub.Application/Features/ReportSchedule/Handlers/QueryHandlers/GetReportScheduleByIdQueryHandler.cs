using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ReportSchedule.DTOs;
using ReportHub.Application.Features.ReportSchedule.Queries;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.ReportSchedule.Handlers.QueryHandlers
{
    public class GetReportScheduleByIdQueryHandler
        (IReportScheduleRepository reportScheduleRepository, IMapper mapper)
        : IRequestHandler<GetReportScheduleByIdQuery, ReportScheduleForGettingDto>
    {
        public async Task<ReportScheduleForGettingDto> Handle(GetReportScheduleByIdQuery request, CancellationToken cancellationToken)
        {
            var reportSchedule = await reportScheduleRepository.Get(r => r.Id == request.Id, cancellationToken);
            if (reportSchedule == null)
                throw new NotFoundException($"Report schedule with id: {request.Id} not found");

            return mapper.Map<ReportScheduleForGettingDto>(reportSchedule);
        }


    }
}
