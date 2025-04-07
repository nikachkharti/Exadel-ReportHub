namespace ReportHub.Identity.Configurations;

public class AuthSettings
{
    public int AccessTokenLifeTimeMinutes { get; set; }
    public string Issuer { get; set; }
}