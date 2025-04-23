using OpenIddict.Server;

namespace ReportHub.Identity.Middlewares;

public class CustomTokenRequestEventHandler :
    IOpenIddictServerHandler<OpenIddictServerEvents.ApplyTokenResponseContext>
{
    public ValueTask HandleAsync(OpenIddictServerEvents.ApplyTokenResponseContext context)
    {
        context.Response["Testing"] = "Now we are working";

        return ValueTask.CompletedTask;
    }
}
