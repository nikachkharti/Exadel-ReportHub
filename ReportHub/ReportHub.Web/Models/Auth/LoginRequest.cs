using Refit;

namespace ReportHub.Web.Models.Auth
{
    public class LoginRequest
    {
        [AliasAs("grant_type")]
        public string GrantType { get; set; }

        [AliasAs("username")]
        public string Username { get; set; }

        [AliasAs("password")]
        public string Password { get; set; }

        [AliasAs("client_id")]
        public string ClientId { get; set; } = "report-hub";

        [AliasAs("client_secret")]
        public string ClientSecret { get; set; } = "client_secret_key";

        [AliasAs("scope")]
        public string Scope { get; set; } = "report-hub-api-scope roles offline_access";
    }
}
