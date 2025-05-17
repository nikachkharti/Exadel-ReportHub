using ReportHub.Web.Services.Auth;
using System.Net.Http.Headers;

namespace ReportHub.Web.Handlers;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public AuthTokenHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}

