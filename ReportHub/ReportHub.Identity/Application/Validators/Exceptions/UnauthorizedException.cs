namespace ReportHub.Identity.Application.Validators.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException()
    {
    }

    public UnauthorizedException(string? message) : base(message)
    {
    }
}
