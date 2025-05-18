using ReportHub.Web.Models.Auth;
using ReportHub.Web.Models.Auth.ViewModels;

namespace ReportHub.Web.Services.Auth
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
    }
}
