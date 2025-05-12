namespace ReportHub.Web.Models.Items
{
    public record ItemForGettingDto(string Id, string Name, string Description, decimal Price, string Currency, bool IsDeleted);
}
