using Refit;

namespace ReportHub.Web.Services.Refit
{
    public interface IPlanApi
    {
        [Delete("/api/plans/{id}")]
        Task DeletePlanAsync(string id);
    }
}
