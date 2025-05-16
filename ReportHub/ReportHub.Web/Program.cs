using ReportHub.Web.Components;
using ReportHub.Web.Extensions;

namespace ReportHub.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddRazorComponents();
            builder.AddRefit();
            builder.AddServices();
            builder.AddSharedStates();
            builder.AddAuthentication();
            builder.AddAuthorization();
            builder.AddCascadingAuthenticationState();
            builder.Services.AddHttpContextAccessor();


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
