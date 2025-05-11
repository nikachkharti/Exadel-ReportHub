namespace ReportHub.Web.Models.Sales
{
    public record SaleForGettingDto(string Id, string ClientId, string ItemId, decimal Amount, DateTime SaleDate, bool IsDeleted);

}
