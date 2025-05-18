using ReportHub.Web.Models.Auth.ViewModels;

namespace ReportHub.Web.Services.Auth;

public interface IAuthServiceWithToken
{
    Task<IList<UserClients>> GetMyClients();

    Task Authorize(string clientId);
}
