using ReportHub.Application.Common.Models;
using ReportHub.Application.Validators.Exceptions;
using System.Net;
using System.Text.Json;

namespace ReportHub.API.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception exception)
    {
        var apiResponse = exception switch
        {
            BadHttpRequestException badRequest => CreateErrorResponse(badRequest, HttpStatusCode.BadRequest),
            InputValidationException validationEx => CreateErrorResponse(validationEx, HttpStatusCode.BadRequest),
            NotFoundException notFoundEx => CreateErrorResponse(notFoundEx, HttpStatusCode.NotFound),
            InternalServerException internalEx => CreateErrorResponse(internalEx, HttpStatusCode.InternalServerError),
            _ => CreateErrorResponse(exception, HttpStatusCode.InternalServerError)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = apiResponse.HttpStatusCode;

        var json = JsonSerializer.Serialize(apiResponse);
        return context.Response.WriteAsync(json);
    }

    private static EndpointResponse CreateErrorResponse(Exception ex, HttpStatusCode statusCode)
    {
        return new EndpointResponse()
        {
            IsSuccess = false,
            HttpStatusCode = (int)statusCode,
            Message = $"{ex.Message}{(ex.InnerException != null ? ": " + ex.InnerException.Message : string.Empty)}",
            Result = null
        };
    }


}
