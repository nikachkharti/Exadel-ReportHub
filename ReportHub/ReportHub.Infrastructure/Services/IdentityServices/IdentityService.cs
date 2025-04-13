using Microsoft.AspNetCore.Http;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Domain.Entities;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;

namespace ReportHub.Infrastructure.Services.IdentityServices;

public class IdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private HttpClient _httpClient;
    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

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
        var result = await _httpClient.PostAsJsonAsync("https://localhost:7171/api/Admin/assign-role", requestBody, cancellationToken);

        return result.IsSuccessStatusCode;
    }

    public async Task<bool> ValidateUserIdExists(string userId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"https://localhost:7171/api/Admin/users/{userId}",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ValidateRoleExists(string role, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"https://localhost:7171/api/Roles/{role}",
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
