using Refit;
using ReportHub.Web.Models;
using ReportHub.Web.Models.Items;

namespace ReportHub.Web.Services.Refit
{
    public interface IItemApi
    {
        [Post("/api/items")]
        Task<EndpointResponse> AddNewItemAsync([Body] CreateItemCommand command);
    }
}
