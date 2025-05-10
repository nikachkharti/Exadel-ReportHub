using ReportHub.Web.Models.Clients;
using ReportHub.Web.Models.Items;
using ReportHub.Web.Models.Plans;
using ReportHub.Web.Models.Sales;

namespace ReportHub.Web.Services.Client
{
    public interface IClientService
    {
        Task<IEnumerable<ClientForGettingDto>> GetClientsAsync(int? page = 1, int? size = 10, string sortBy = "", bool ascending = true);
        Task<IEnumerable<ItemForGettingDto>> GetItemsOfClientAsync(string clientId, int? page = 1, int? size = 10, string sortBy = "", bool ascending = true);
        Task<IEnumerable<PlanForGettingDto>> GetPlansOfClientAsync(string clientId, int? page = 1, int? size = 10, string sortBy = "", bool ascending = true);
        Task<IEnumerable<SaleForGettingDto>> GetSalesOfClientAsync(string clientId, int? page = 1, int? size = 10, string sortBy = "", bool ascending = true);
    }
}
