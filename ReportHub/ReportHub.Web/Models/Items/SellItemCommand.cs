namespace ReportHub.Web.Models.Items
{
    public record SellItemCommand
    (
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime SaleDate
    );
}
