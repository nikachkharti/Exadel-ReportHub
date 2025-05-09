using Refit;
using ReportHub.Web.Models;

namespace ReportHub.Web.Services.Refit
{
    public interface IClientApi
    {
        [Get("/api/clients")]
        Task<EndpointResponse> GetAllClientsAsync(
            int? pageNumber = 1,
            int? pageSize = 10,
            string sortingParameter = "",
            bool ascending = true);
    }
}
