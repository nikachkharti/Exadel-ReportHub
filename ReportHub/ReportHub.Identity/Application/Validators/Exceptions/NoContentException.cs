namespace ReportHub.Identity.Application.Validators.Exceptions;

public class NoContentException : Exception
{
    public NoContentException()
    {
    }
    public NoContentException(string message) : base(message)
    {
    }
    public NoContentException(string entityName, object key)
        : base($"{entityName} with {key} has no content")
    {
    }
}
