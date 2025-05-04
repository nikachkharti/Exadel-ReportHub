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

            var planExpireJobKey = new JobKey("PlanExpireJob");
            var dynamicReportScheduleJobKey = new JobKey("DynamicReportScheduleJob");

            q.AddJob<PlanExpireJob>(opts => opts.WithIdentity(planExpireJobKey));
            q.AddJob<DynamicReportScheduleJob>(opts => opts.WithIdentity(dynamicReportScheduleJobKey));

            q.AddTrigger(opts => opts
                .ForJob(planExpireJobKey)
                .WithIdentity("PlanExpireTrigger")
                .WithCronSchedule("0 0 * * * ?") // Every hour
            );

            q.AddTrigger(opts => opts
                .ForJob(dynamicReportScheduleJobKey)
                .WithIdentity("DynamicReportScheduleJob")
                .StartNow() //When app starts
            );
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });


        //Custom Services
        services.AddSingleton<ISmtpClientWrapper, SmtpClientWrapper>();
        services.AddSingleton<IEmailService, EmailService>();
    }
}
