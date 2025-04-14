using FluentValidation;
using Microsoft.AspNetCore.Http;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.Item.Commands;
using System.Security.Claims;

namespace ReportHub.Application.Validators.ItemValidators.ItemCommandValidators;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator(IClientRepository clientRepository, 
        IHttpContextAccessor httpContextAccessor,
        IClientUserRepository clientUserRepository)
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required.")
            .MustAsync(async (string clientId, CancellationToken cancellationToken) =>
            {
                return await clientRepository.Get(c => c.Id == clientId) is not null;
            })
            .WithMessage("Client does not exist")
            .MustAsync(async (string clientId, CancellationToken cancellationToken) =>
            {
                var roles = httpContextAccessor.HttpContext?.User.FindFirst("role")?.Value;
                
                if (roles.Contains("SuperAdmin"))return true;
                
                var userId = httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;

                var role = await clientUserRepository.Get(c => c.ClientId == clientId && c.UserId == userId);

                if (role is null) return false;

                return role.Role.Equals("ClientAdmin") || role.Role.Equals("Admin");
            })
            .WithMessage("You do not have accecc to create item for this client");


        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(120).WithMessage("Description must not exceed 120 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code (e.g., USD, EUR).");
    }
}
