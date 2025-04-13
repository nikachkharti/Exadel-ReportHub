﻿namespace ReportHub.Application.Contracts.IdentityContracts;

public interface IIdentityService
{
    Task<bool> AssignUserRole(string userId, string roleName);

    Task<bool> ValidateUserIdExists(string userId, CancellationToken cancellationToken);

    Task<bool> ValidateRoleExists(string role, CancellationToken cancellationToken);
}
