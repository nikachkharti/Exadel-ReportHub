namespace ReportHub.Web.Models.Items
{
    public record CreateItemCommand(string ClientId, string Name, string Description, decimal Price, string Currency);

}
