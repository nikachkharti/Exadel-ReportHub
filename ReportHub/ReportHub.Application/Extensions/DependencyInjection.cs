using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddSingleton<ISmtpClientWrapper, SmtpClientWrapper>();
        services.AddSingleton<IEmailService, EmailService>();

        services.AddHostedService<PlanExpireWorker>();
    }
}
