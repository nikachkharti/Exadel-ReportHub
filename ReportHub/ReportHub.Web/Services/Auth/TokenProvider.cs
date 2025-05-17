using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ReportHub.Web.Services.Auth;

public class TokenProvider : ITokenProvider
{
    private readonly ProtectedSessionStorage _storage;
    private readonly NavigationManager _nav;

    public TokenProvider(ProtectedSessionStorage storage, NavigationManager nav)
    {
        _storage = storage;
        _nav = nav;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var result = await _storage.GetAsync<string>("authToken");
        return result.Success ? result.Value : null;
    }

    public async Task SetAccessTokenAsync(string token)
    {
        if (_nav.Uri.Contains("__prerender"))
            return;

        await _storage.SetAsync("authToken", token);
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        var result = await _storage.GetAsync<string>("refresh_token");
        return result.Success ? result.Value : null;
    }

    public async Task SetRefreshTokenAsync(string token)
    {
        await _storage.SetAsync("refresh_token", token);
    }
}
