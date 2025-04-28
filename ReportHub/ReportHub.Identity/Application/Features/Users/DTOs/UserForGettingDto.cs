namespace ReportHub.Identity.Application.Features.Users.DTOs;

public class UserForGettingDto
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
