using Refit;
using ReportHub.Web.Models.Auth.ViewModels;

namespace ReportHub.Web.Services.Refit;

public interface IAuthApiWithToken
{
    [Get("/my-clients")]
    Task<IList<UserClients>> GetMyClientsAsync();
}
