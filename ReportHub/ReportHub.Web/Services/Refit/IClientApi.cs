using Refit;
using ReportHub.Web.Models;

namespace ReportHub.Web.Services.Refit
{
    public interface IClientApi
    {
        [Get("/api/clients")]
        Task<EndpointResponse> GetAllClientsAsync(
            [AliasAs("pageNumber")] int? pageNumber = 1,
            [AliasAs("pageSize")] int? pageSize = 10,
            [AliasAs("sortingParameter")] string sortingParameter = "",
            [AliasAs("ascending")] bool ascending = true);
    }
}
