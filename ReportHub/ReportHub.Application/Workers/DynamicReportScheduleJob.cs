using Quartz;
using ReportHub.Application.Contracts.RepositoryContracts;

namespace ReportHub.Application.Workers
{
    public class DynamicReportScheduleJob(
        IReportScheduleRepository reportScheduleRepository,
        ISchedulerFactory schedulerFactory) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var reportSchedules = await reportScheduleRepository.GetAll();

            foreach (var schedule in reportSchedules/*.Where(s => s.IsActive)*/)
            {
                var jobKey = new JobKey($"ExecuteReportJob_{schedule.CustomerId}_{schedule.Id}");

                if (await scheduler.CheckExists(jobKey))
                    continue;

                var jobDetail = JobBuilder.Create<ExecuteReportJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("CustomerId", schedule.CustomerId)
                    .UsingJobData("ReportId", schedule.Id.ToString())
                    .StoreDurably()
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"Trigger_{schedule.CustomerId}_{schedule.Id}")
                    .WithCronSchedule(schedule.CronExpression)
                    .ForJob(jobDetail)
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);
            }
        }
    }
}
