using Quartz;
using ReportHub.Application.Contracts.RepositoryContracts;

namespace ReportHub.Application.Workers
{
    public class DynamicReportScheduleJob(IReportScheduleRepository reportScheduleRepository) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var reportSchedules = await reportScheduleRepository.GetAll(/*r => r.IsActive*/);

            if (reportSchedules.Any())
            {
                foreach (var schedule in reportSchedules)
                {
                    // each schedule has this data format,
                    // I want to execute background job independently for each
                    // schedule and send notification on email. Each of these jobs should be trigerred independently
                    // based on their own cron expression. you can miss actual implementation of email sending service just focus on jobs

                    /*
                     {
                          "_id": {
                            "$oid": "68172ddef0120544a40098dc"
                          },
                          "IsDeleted": false,
                          "CustomerId": "67fa2d8114e2389cd8064458",
                          "CronExpression": "0 0 9 ? * MON",
                          "Format": 0,
                          "IsActive": false
                        }
                     */
                }
            }
        }
    }
}
