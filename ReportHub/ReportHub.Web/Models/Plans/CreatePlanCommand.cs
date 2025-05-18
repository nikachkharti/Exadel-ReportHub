namespace ReportHub.Web.Models.Plans
{
    public record CreatePlanCommand
    (
        string ClientId,
        string ItemId,
        decimal Amount,
        DateTime StartDate,
        DateTime EndDate,
        PlanStatus Status
    );
}
