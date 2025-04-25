namespace ReportHub.Identity.Models;

public record ErrorResponse(bool IsSuccess, int HttpStatusCode, string Message, IEnumerable<string>? Errors);
