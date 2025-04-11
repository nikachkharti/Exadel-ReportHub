namespace ReportHub.Identity.Models;

public class AssignRoleRequest
{
    public string Username { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;
}
