using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Workers
{
    public partial class DataSeeder
    {
        private async Task SeedReportSchedulesAsync(IServiceScope scope, CancellationToken cancellationToken)
        {
            var reportScheduleRepository = scope.ServiceProvider.GetRequiredService<IReportScheduleRepository>();

            var existingSchedules = await reportScheduleRepository.GetAll(pageNumber: 1, pageSize: 1);

            if (existingSchedules.Any()) return;

            var schedules = GetReportSchedules();

            await reportScheduleRepository.InsertMultiple(schedules, cancellationToken);
        }
        private IEnumerable<ReportSchedule> GetReportSchedules()
        {
            return new List<ReportSchedule>()
            {
                new ReportSchedule()
                {
                    Id = "68172ddef0120544a40098db",
                    CustomerId = "67fa2d8114e2389cd8064457", // John Doe
                    IsDeleted = false,
                    CronExpression = "0 0/15 * * * ?", //Every 15 minutes
                    //CronExpression = "0/15 * * * * ?", //Every 15 second
                    IsActive = false,
                    Format = ReportFormat.CSV
                },
                new ReportSchedule()
                {
                    Id = "68172ddef0120544a40098dc",
                    CustomerId = "67fa2d8114e2389cd8064458", // Bill Butcher
                    IsDeleted = false,
                    //CronExpression = "0 0 9 ? * MON", //Every Monday at 9:00 AM
                    CronExpression = "0/10 * * * * ?", //Every 10 second
                    IsActive = false,
                    Format = ReportFormat.CSV
                },
                new ReportSchedule()
                {
                    Id = "68172ddef0120544a40098dd",
                    CustomerId = "67fa2d8114e2389cd8064459", //Freddy Kruger
                    IsDeleted = false,
                    CronExpression = "0 0 9 ? * MON", //Every Monday at 9:00 AM
                    IsActive = false,
                    Format = ReportFormat.PDF
                },
                new ReportSchedule()
                {
                    Id = "68172ddef0120544a40098de",
                    CustomerId = "67fa2d8114e2389cd806445a", // John Cenna
                    IsDeleted = false,
                    CronExpression = "0 0 9 ? * MON", //Every Monday at 9:00 AM
                    IsActive = false,
                    Format = ReportFormat.PDF
                }
            };
        }
    }


}
