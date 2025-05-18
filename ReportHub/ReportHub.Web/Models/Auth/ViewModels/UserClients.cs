using Refit;

namespace ReportHub.Web.Models.Auth.ViewModels;

public class UserClients
{
    [AliasAs("id")]
    public string Id { get; set; }

    [AliasAs("clientId")]
    public string ClientId { get; set; }

    [AliasAs("role")]
    public string Role { get; set; }
}
