namespace ReportHub.Web.Services.Plan
{
    public interface IPlanService
    {
        Task<bool> DeletePlanAsync(string planId);
    }
}
