using Refit;
using ReportHub.Web.Models.Auth;
using ReportHub.Web.Models.Auth.ViewModels;

namespace ReportHub.Web.Services.Refit
{
    public interface IAuthApi
    {
        [Post("/auth/login")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<TokenResponse> LoginAsync([Body(BodySerializationMethod.UrlEncoded)] LoginRequest request);
    }
}
