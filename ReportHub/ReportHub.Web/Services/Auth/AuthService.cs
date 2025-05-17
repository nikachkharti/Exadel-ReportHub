using ReportHub.Web.Models.Auth;
using ReportHub.Web.Services.Refit;

namespace ReportHub.Web.Services.Auth
{
    public class AuthService(IAuthApi authApi, ITokenProvider tokenProvider) : IAuthService
    {
        private string _accessToken;

        public string _refreshToken;
        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            request.GrantType = "password";
            var result = await authApi.LoginAsync(request);

            await tokenProvider.SetAccessTokenAsync(result.AccessToken);
            await tokenProvider.SetRefreshTokenAsync(result.RefreshToken);

            return result;
        }
    }
}
