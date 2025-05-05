using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ReportSchedule.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.ReportSchedule.Handlers.CommandHandlers
{
    public class UpdateReportScheduleCommandHandler
        (IReportScheduleRepository reportScheduleRepository, ICustomerRepository customerRepository, IMapper mapper)
        : IRequestHandler<UpdateReportScheduleCommand, string>
    {
        public async Task<string> Handle(UpdateReportScheduleCommand request, CancellationToken cancellationToken)
        {
            await ValidateReportScheduleExistsWithUser(request, cancellationToken);

            var updatedDocument = mapper.Map<Domain.Entities.ReportSchedule>(request);

            await reportScheduleRepository.UpdateSingleDocument(r => r.Id == request.Id, updatedDocument);
            return request.Id;
        }

        private async Task ValidateReportScheduleExistsWithUser(UpdateReportScheduleCommand request, CancellationToken cancellationToken)
        {
            var reportSchedule = await reportScheduleRepository.Get(r => r.Id == request.Id, cancellationToken);

            if (reportSchedule == null)
                throw new NotFoundException($"Report schedule with id {request.Id} not found");

            var customer = await customerRepository.Get(c => c.Id == reportSchedule.CustomerId, cancellationToken);
            if (customer == null)
                throw new NotFoundException($"Customer with id: {reportSchedule.CustomerId} not found. Report schedule can't be deleted for unexisted customer");
        }

    }
}
