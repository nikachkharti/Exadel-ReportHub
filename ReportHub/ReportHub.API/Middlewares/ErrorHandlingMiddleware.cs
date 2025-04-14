using ReportHub.Application.Validators.Exceptions;

namespace ReportHub.API.Middlewares;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);

        }
        catch (InputValidationException inputException)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(inputException.Errors);
        }

        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong.");
        }
    }
}
