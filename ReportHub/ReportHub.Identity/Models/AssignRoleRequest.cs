﻿namespace ReportHub.Identity.Models;

public class AssignRoleRequest
{
    public string UserId { get; set; } = string.Empty;

    public string RoleName { get; set; } = string.Empty;
}
