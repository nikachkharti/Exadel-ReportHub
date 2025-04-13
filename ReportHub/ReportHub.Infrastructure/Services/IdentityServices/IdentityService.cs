using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ReportHub.Application.Contracts.IdentityContracts;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ReportHub.Infrastructure.Services.IdentityServices;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _identitBaseyUrl;
    private HttpClient _httpClient;
    public IdentityService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _identitBaseyUrl = configuration["Authentication:Issuer"] ?? 
                                    throw new ArgumentNullException("Identity url is not set");
        _httpClient = new HttpClient();

        var token = GetBearerTokenFromRequest();
        
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<bool> AssignUserRole(string userId, string roleName, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            userId = userId,
            roleName = roleName
        };
        var result = await _httpClient.PostAsJsonAsync($"{_identitBaseyUrl}/api/Admin/assign-role", requestBody, cancellationToken);

        return result.IsSuccessStatusCode;
    }

    public async Task<bool> ValidateUserIdExists(string userId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"{_identitBaseyUrl}/api/Admin/users/{userId}",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ValidateRoleExists(string role, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"{_identitBaseyUrl}/api/Roles/{role}",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }

    private string GetBearerTokenFromRequest()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader) ||
            !authorizationHeader.StartsWith("Bearer "))
            return null;

        return authorizationHeader.Substring("Bearer ".Length).Trim();
    }
}
