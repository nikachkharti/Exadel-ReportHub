namespace ReportHub.Web.Services.Auth;

public interface ITokenProvider
{
    Task<string> GetAccessTokenAsync();
    Task<string> GetRefreshTokenAsync();

    Task SetAccessTokenAsync(string token);

    Task SetRefreshTokenAsync(string token);

}
