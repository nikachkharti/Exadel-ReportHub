using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ReportHub.Identity.Application.Features.Users.Commands;
using ReportHub.Identity.Domain.Entities;
using ReportHub.Identity.Validators.Exceptions;

namespace ReportHub.Identity.Application.Features.Users.Handlers.CommandHandlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(UserManager<User> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {

        await ThrowIfEmailExistsAsync(request);
        await ThrowIfUsernameExistsAsync(request);

        var user = _mapper.Map<User>(request);

        return await CreateUserAsync(request, user);
    }

    private async Task<string> CreateUserAsync(CreateUserCommand request, User user)
    {
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new Exception("User creation failed.");

        return user.Id;
    }

    private async Task ThrowIfEmailExistsAsync(CreateUserCommand user)
    {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);
        if (existingUser is not null)
            throw new ConflictException(nameof(User), user.Email);
    }

    private async Task ThrowIfUsernameExistsAsync(CreateUserCommand user)
    {
        var existingUser = await _userManager.FindByNameAsync(user.Username);

        if (existingUser is not null)
            throw new ConflictException(nameof(User), user.Username);
    }
}
