namespace ReportHub.Identity.Application.Interfaces.ServiceInterfaces;

public interface IRequestContextService
{
    string GetUserId();

    string GetClientId();

    string GetRole();
}
