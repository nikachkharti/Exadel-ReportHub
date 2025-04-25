namespace ReportHub.Identity.Interfaces.ServiceInterfaces;

public interface IRequestContextService
{
    string GetUserId();

    string GetClientId();

    string GetRole();
}
