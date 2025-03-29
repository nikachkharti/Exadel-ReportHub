namespace ReportHub.Application.Validation.Exceptions;

public class InputValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public InputValidationException(IReadOnlyDictionary<string, string[]> errors) :
        base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}
