using FluentValidation;
using ReportHub.Identity.Models;
using ReportHub.Identity.Validators.Exceptions;
using System.Net;
using System.Text.Json;

namespace ReportHub.Identity.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = ex switch
        {
            NoContentException noContent => CreateErrorResponse(HttpStatusCode.NoContent, noContent.Message),
            NotFoundException notFound => CreateErrorResponse(HttpStatusCode.NotFound, notFound.Message),
            ValidationException validation => CreateErrorResponse(HttpStatusCode.BadRequest, validation.Message, validation.Errors.Select(x => x.ErrorMessage)),
            ConflictException conflictException => CreateErrorResponse(HttpStatusCode.Conflict, conflictException.Message),
            _ => CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.HttpStatusCode;

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }

    private ErrorResponse CreateErrorResponse(HttpStatusCode statusCode, string message, IEnumerable<string> errors = default!)
    {
        return new ErrorResponse(false, (int)statusCode, message, errors);
    }

}
