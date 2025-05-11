using ReportHub.Web.Services.Refit;

namespace ReportHub.Web.Services.Plan
{
    public class PlanService(IPlanApi planApi) : IPlanService
    {
        public async Task<bool> DeletePlanAsync(string planId)
        {
            try
            {
                await planApi.DeletePlanAsync(planId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
