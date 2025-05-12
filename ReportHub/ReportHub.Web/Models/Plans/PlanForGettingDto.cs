namespace ReportHub.Web.Models.Plans
{
    public record PlanForGettingDto
    (
        string Id,
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime StartDate,
        DateTime EndDate,
        PlanStatus Status,
        bool IsDeleted
    );
}
