namespace ReportHub.API.Authorization.Permissions;

public enum PermissionType
{
    CreateClient,
    GetAllClients,
    GetOneClient,
    UpdateClient,
    DeleteClient,

    AddUserToClient,
    GetClientUsers,
    RemoveUserFromClient,

    CreateItem,
    GetAllItems,
    GetClientItems,
    UpdateItem,
    DeleteItem,

    CreateInvoice,
    GetAllInvoices,
    GetClientInvoices,
    UpdateInvoice,
    DeleteInvoice,

    CreateCustomer,
    GetAllCustomers,
    GetClientCustomers,
    UpdateCustomer,
    DeleteCustomer,

    CreatePlan,
    GetAllPlans,
    GetClientPlans,
    UpdatePlan,
    DeletePlan,

    CreateReport,
    GetAllReports,
    GetClientReports,
    UpdateReport,
    DeleteReport,
}
