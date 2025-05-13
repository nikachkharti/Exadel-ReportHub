using ReportHub.Web.Models.Auth;

namespace ReportHub.Web.Services.Auth
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
    }
}
