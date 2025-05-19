using Refit;
using ReportHub.Web.Models.Plans;

namespace ReportHub.Web.Services.Refit
{
    public interface IPlanApi
    {
        [Delete("/api/plans/{id}")]
        Task DeletePlanAsync(string id);

        [Post("/api/plans")]
        Task<bool> AddNewPlanAsync(CreatePlanCommand command);
    }
}
