namespace ReportHub.API.Authorization.Permissions;

public static class RolePermissions
{
    private readonly static Dictionary<string, HashSet<PermissionType>> _rolePermissions = new()
    {
        [ "SuperAdmin" ] = new()
        {
            PermissionType.CreateClient, PermissionType.GetAllClients, PermissionType.GetOneClient, PermissionType.UpdateClient, PermissionType.DeleteClient,
            PermissionType.AddUserToClient, PermissionType.GetAllUsersInClient, PermissionType.RemoveUserFromClient,
        },

        [ "Owner" ] = new HashSet<PermissionType>
        {
            PermissionType.GetAllClients, PermissionType.GetOneClient, PermissionType.UpdateClient, 
            PermissionType.AddUserToClient, PermissionType.GetAllUsersInClient, PermissionType.RemoveUserFromClient,
            PermissionType.CreateItem, PermissionType.GetClientItems, PermissionType.UpdateItem, PermissionType.DeleteItem,
            PermissionType.CreateInvoice, PermissionType.GetClientInvoices, PermissionType.UpdateInvoice, PermissionType.DeleteInvoice,
            PermissionType.CreateCustomer, PermissionType.GetClientCustomers, PermissionType.UpdateCustomer, PermissionType.DeleteCustomer,
            PermissionType.CreatePlan, PermissionType.GetClientPlans, PermissionType.UpdatePlan, PermissionType.DeletePlan,
            PermissionType.CreateReport, PermissionType.GetClientReports, PermissionType.UpdateReport, PermissionType.DeleteReport,
        },

        [ "ClientAdmin" ] = new()
        {
            PermissionType.GetAllClients, PermissionType.GetOneClient,
            PermissionType.AddUserToClient, PermissionType.RemoveUserFromClient,
            PermissionType.CreateItem, PermissionType.GetClientItems, PermissionType.UpdateItem, PermissionType.DeleteItem,
            PermissionType.CreateInvoice, PermissionType.GetClientInvoices, PermissionType.UpdateInvoice,
            PermissionType.CreateCustomer, PermissionType.GetClientCustomers, PermissionType.UpdateCustomer,
            PermissionType.CreatePlan, PermissionType.GetClientPlans, PermissionType.UpdatePlan,
            PermissionType.CreateReport, PermissionType.GetClientReports,
        },

        [ "Operator" ] = new()
        {
            PermissionType.GetAllClients, PermissionType.GetOneClient,
            PermissionType.GetAllUsersInClient,
            PermissionType.GetClientItems,
            PermissionType.CreateInvoice, PermissionType.GetClientInvoices,
            PermissionType.CreateCustomer, PermissionType.GetClientCustomers
        }
    };

    public static bool HasPermission(string role, PermissionType permission)
    {
        return _rolePermissions.TryGetValue(role, out var permissions) && permissions.Contains(permission);
    }
}
