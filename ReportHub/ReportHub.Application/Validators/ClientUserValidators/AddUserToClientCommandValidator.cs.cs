using FluentValidation;
using Microsoft.AspNetCore.Http;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Commands;
using System.Net.Http.Headers;

namespace ReportHub.Application.Validators.ClientUserValidators;

public class AddUserToClientCommandValidator : AbstractValidator<AddUserToClientCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IClientRepository _clientRepository;
    private readonly IClientUserRepository _clientUserRepository;

    public AddUserToClientCommandValidator(
        IHttpContextAccessor httpContextAccessor, IClientRepository clientRepository, IClientUserRepository clientUserRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _clientRepository = clientRepository;
        _clientUserRepository = clientUserRepository;

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required.")
            .MustAsync(ValidateClientExist)
            .WithMessage("Client does not exist"); ;


        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.")
            .MustAsync(ValidateUserIdExists)
            .WithMessage("User does not exist or you have no access to assign role");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required.")
            .MustAsync(ValidateRoleExists)
            .WithMessage("Role does not exist or you do not have access to give role");

        RuleFor(x => x)
            .MustAsync(EnsureUserRoleNotAssigned)
            .WithMessage("This user has already role for this client");
    }

    private async Task<bool> EnsureUserRoleNotAssigned(AddUserToClientCommand a, CancellationToken cancellationToken)
    {
        return await _clientUserRepository.Get(c => c.UserId == a.UserId && c.ClientId == a.ClientId, cancellationToken) is null;
    }

    private async Task<bool> ValidateClientExist(string clientId, CancellationToken cancellationToken)
    {
        return await _clientRepository.Get(c => c.Id == clientId, cancellationToken) is not null;
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
