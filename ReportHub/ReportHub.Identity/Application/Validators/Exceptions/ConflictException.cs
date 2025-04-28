namespace ReportHub.Identity.Application.Validators.Exceptions;

public class ConflictException : Exception
{
    public ConflictException()
    {
    }

    public ConflictException(string? message) : base(message)
    {
    }

    public ConflictException(string entityName, object key)
        : base($"{entityName} with {key} already exist")
    {
    }
}
