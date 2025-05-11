using Refit;
using ReportHub.Web.Models;
using ReportHub.Web.Models.Clients;
using ReportHub.Web.Models.Items;

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

        [Get("/api/clients/{clientId}/items")]
        Task<EndpointResponse> GetItemsOfClientAsync(
            string clientId,
            int? pageNumber = 1,
            int? pageSize = 10,
            string sortingParameter = "",
            bool ascending = true);

        [Get("/api/clients/{clientId}/plans")]
        Task<EndpointResponse> GetPlansOfClientAsync(
            string clientId,
            int? pageNumber = 1,
            int? pageSize = 10,
            string sortingParameter = "",
            bool ascending = true);

        [Get("/api/clients/{clientId}/sales")]
        Task<EndpointResponse> GetSalesOfClientAsync(
            string clientId,
            int? pageNumber = 1,
            int? pageSize = 10,
            string sortingParameter = "",
            bool ascending = true);

        [Delete("/api/clients/{clientId}/items/{itemId}")]
        Task DeleteItemOfClientAsync(string clientId, string itemId);

        [Post("/api/clients/sell")]
        Task<EndpointResponse> SellItemAsync([Body] SellItemCommand command);

        [Post("/api/clients")]
        Task<EndpointResponse> AddNewClientAsync([Body] CreateClientCommand command);
    }
}
