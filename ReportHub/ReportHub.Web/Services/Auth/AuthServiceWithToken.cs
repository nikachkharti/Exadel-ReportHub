using Microsoft.Extensions.Configuration;
using ReportHub.Web.Models.Auth.ViewModels;
using ReportHub.Web.Services.Refit;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ReportHub.Web.Services.Auth;

public class AuthServiceWithToken(IConfiguration configuration, ITokenProvider tokenProvider) : IAuthServiceWithToken
{
    private readonly HttpClient _client = new HttpClient()
    {
        BaseAddress = new Uri(configuration.GetValue<string>("Refit:BaseAddressAuth"))
    };

    public async Task<IList<UserClients>> GetMyClients()
    {
        _client.DefaultRequestHeaders.Authorization =new AuthenticationHeaderValue("Bearer", await tokenProvider.GetAccessTokenAsync());
        var response = await _client.GetAsync("/my-clients");

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<IList<UserClients>>(body, option);

            return result;
        }
        else
        {
            throw new Exception("Failed to fetch clients");
        }
    }

    public async Task Authorize(string clientId)
    {
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await tokenProvider.GetAccessTokenAsync());

        var request = new HttpRequestMessage(HttpMethod.Post, "/auth/select-client")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = "report-hub",
                ["client_secret"] = "client_secret_key",
                ["scope"] = "report-hub-api-scope roles offline_access",
                ["refresh_token"] = await tokenProvider.GetRefreshTokenAsync(),
                ["userClientId"] = clientId
            })
        };

        var response = await _client.SendAsync(request);

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var clientToken = json.RootElement.GetProperty("access_token").GetString();
        var refreshToken = json.RootElement.GetProperty("refresh_token").GetString();

        await tokenProvider.SetRefreshTokenAsync(refreshToken);
        await tokenProvider.SetAccessTokenAsync(clientToken);

    }
}
