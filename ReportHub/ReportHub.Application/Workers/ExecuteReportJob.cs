using Quartz;
using ReportHub.Application.Contracts.Notification;
using ReportHub.Application.Contracts.RepositoryContracts;
using Serilog;

namespace ReportHub.Application.Workers
{
    /// <summary>
    ///  Job that will be executed per schedule for each schedule using Quartz.NET, 
    ///  based on their individual CronExpression and send report email
    /// </summary>
    public class ExecuteReportJob(IEmailService emailService, ICustomerRepository customerRepository) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var customerId = context.MergedJobDataMap.GetString("CustomerId");
            var reportId = context.MergedJobDataMap.GetString("ReportId");

            Log.Information($"[ExecuteReportJob] Running for ReportId: {reportId}, CustomerId: {customerId}");

            var customer = await customerRepository.Get(c => c.Id == customerId);

            if (customer != null)
            {
                await emailService.Send(to: customer.Email, "TEST", "TEST");
                Log.Information($"[ExecuteReportJob] Email sending process completed for {customer.Email}");
            }

            await Task.CompletedTask;
        }
    }
}
