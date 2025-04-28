namespace ReportHub.Application.Contracts.IdentityContracts;

public interface IRequestContextService
{
    string GetUserRole();

    string GetClientId();
}
