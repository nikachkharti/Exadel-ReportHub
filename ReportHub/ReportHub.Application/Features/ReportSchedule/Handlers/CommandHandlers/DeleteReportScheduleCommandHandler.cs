using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ReportSchedule.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.ReportSchedule.Handlers.CommandHandlers
{
    public class DeleteReportScheduleCommandHandler
        (IReportScheduleRepository reportScheduleRepository, ICustomerRepository customerRepository)
        : IRequestHandler<DeleteReportScheduleCommand, string>
    {
        public async Task<string> Handle(DeleteReportScheduleCommand request, CancellationToken cancellationToken)
        {
            await ValidateReportScheduleExistsWithUser(request, cancellationToken);
            await reportScheduleRepository.Delete(r => r.Id == request.Id, cancellationToken);
            return request.Id;
        }

        private async Task ValidateReportScheduleExistsWithUser(DeleteReportScheduleCommand request, CancellationToken cancellationToken)
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
