using FluentValidation;
using Microsoft.AspNetCore.Http;
using ReportHub.Application.Features.CLientUsers.Commands;
using System.Net.Http.Headers;

namespace ReportHub.Application.Validators.ClientUserValidators;

public class AddUserToClientCommandValidator : AbstractValidator<AddUserToClientCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddUserToClientCommandValidator(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.");

        RuleFor(x => x.UserId)
            .MustAsync(ValidateUserIdExists)
            .WithMessage("User does not exist or you have no access to assign role");

        RuleFor(x => x.Role)
            .MustAsync(ValidateRoleExists)
            .WithMessage("Role does not exist or you do not have access to give role");
    }

    private async Task<bool> ValidateUserIdExists(string userId, CancellationToken cancellationToken)
    {
        var token = GetBearerTokenFromRequest();
        if (string.IsNullOrEmpty(token))
            return false;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(
            $"https://localhost:7171/api/Admin/users/{userId}",
            cancellationToken);

        return response.IsSuccessStatusCode;
    }

    private async Task<bool> ValidateRoleExists(string role, CancellationToken cancellationToken)
    {
        var token = GetBearerTokenFromRequest();
        if (string.IsNullOrEmpty(token))
            return false;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(
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
