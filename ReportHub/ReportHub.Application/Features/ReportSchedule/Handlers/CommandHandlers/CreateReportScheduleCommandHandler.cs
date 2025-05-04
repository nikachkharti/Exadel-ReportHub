using AutoMapper;
using MediatR;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.ReportSchedule.Commands;
using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.Application.Features.ReportSchedule.Handlers.CommandHandlers
{
    public class CreateReportScheduleCommandHandler(IReportScheduleRepository reportScheduleRepository, ICustomerRepository customerRepository, IMapper mapper)
        : IRequestHandler<CreateReportScheduleCommand, string>
    {
        public async Task<string> Handle(CreateReportScheduleCommand request, CancellationToken cancellationToken)
        {
            await ValidateCustomerExists(request);

            var reportSchedule = mapper.Map<Domain.Entities.ReportSchedule>(request);
            await reportScheduleRepository.Insert(reportSchedule);
            return reportSchedule.Id;
        }

        private async Task ValidateCustomerExists(CreateReportScheduleCommand request)
        {
            var customer = await customerRepository.Get(r => r.Id == request.CustomerId);
            if (customer == null)
                throw new NotFoundException($"Customer with id: {request.CustomerId} not found. Report schedule can't be added for unexisted customer");
        }
    }
}
