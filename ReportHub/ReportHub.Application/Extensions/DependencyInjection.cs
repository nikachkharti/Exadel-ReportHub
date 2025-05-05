using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Simpl;
using ReportHub.Application.Contracts.Notification;
using ReportHub.Application.Features.Notification;
using ReportHub.Application.Validators;
using ReportHub.Application.Workers;
namespace ReportHub.Application.Extensions;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddAutoMapper(applicationAssembly);

        services.AddHttpContextAccessor();

        //Quartz
        services.AddQuartz(q =>
        {
            q.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();

            // Schedule the recurring job to scan all dynamic report schedules
            var dynamicReportScheduleJobKey = new JobKey("DynamicReportScheduleJob");
            var planExpireJobKey = new JobKey("PlanExpireJob");

            q.AddJob<DynamicReportScheduleJob>(opts => opts.WithIdentity(dynamicReportScheduleJobKey));
            q.AddTrigger(opts => opts
                .ForJob(dynamicReportScheduleJobKey)
                .WithIdentity("DynamicReportScheduleTrigger")
                .StartNow()
            );

            q.AddJob<PlanExpireJob>(opts => opts.WithIdentity(planExpireJobKey)); // optional
            q.AddTrigger(opts => opts
                .ForJob(planExpireJobKey)
                .WithIdentity("PlanExpireTrigger")
                .WithCronSchedule("0 0 * * * ?") // Every hour
            );

            // Register the job to allow dynamic creation (no trigger here)
            q.AddJob<ExecuteReportJob>(opts => opts.StoreDurably());
        });

        // Quartz Hosted Service
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });


        //Custom Services
        services.AddTransient<ISmtpClientWrapper, SmtpClientWrapper>();
        services.AddTransient<IEmailService, EmailService>();
    }
}
