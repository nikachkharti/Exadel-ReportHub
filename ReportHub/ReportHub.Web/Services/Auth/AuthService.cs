using ReportHub.Web.Models.Auth;
using ReportHub.Web.Services.Refit;

namespace ReportHub.Web.Services.Auth
{
    public class AuthService(IAuthApi authApi) : IAuthService
    {
        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            request.GrantType = "password";
            var result = await authApi.LoginAsync(request);

            return result;
        }
    }
}
