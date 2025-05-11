using Newtonsoft.Json;
using ReportHub.Web.Models.Clients;
using ReportHub.Web.Models.Items;
using ReportHub.Web.Models.Plans;
using ReportHub.Web.Models.Sales;
using ReportHub.Web.Services.Refit;

namespace ReportHub.Web.Services.Client
{
    public class ClientService(IClientApi clientApi) : IClientService
    {
        public async Task<IEnumerable<ClientForGettingDto>> GetClientsAsync(int? page = 1, int? size = 10, string sortBy = "", bool ascending = true)
        {
            var response = await clientApi.GetAllClientsAsync(page, size, sortBy, ascending);

            if (!response.IsSuccess)
            {
                return Enumerable.Empty<ClientForGettingDto>();
            }

            return JsonConvert.DeserializeObject<IEnumerable<ClientForGettingDto>>(response.Result.ToString());
        }

        public async Task<IEnumerable<ItemForGettingDto>> GetItemsOfClientAsync(string clientId, int? page = 1, int? size = 10, string sortBy = "", bool ascending = true)
        {
            var resposne = await clientApi.GetItemsOfClientAsync(clientId, page, size, sortBy, ascending);

            if (!resposne.IsSuccess)
            {
                return Enumerable.Empty<ItemForGettingDto>();
            }

            return
                JsonConvert.DeserializeObject<IEnumerable<ItemForGettingDto>>(resposne.Result.ToString())
                .Where(x => x.IsDeleted == false);
        }

        public async Task<IEnumerable<PlanForGettingDto>> GetPlansOfClientAsync(string clientId, int? page = 1, int? size = 10, string sortBy = "", bool ascending = true)
        {
            var resposne = await clientApi.GetPlansOfClientAsync(clientId, page, size, sortBy, ascending);

            if (!resposne.IsSuccess)
            {
                return Enumerable.Empty<PlanForGettingDto>();
            }

            return
                JsonConvert.DeserializeObject<IEnumerable<PlanForGettingDto>>(resposne.Result.ToString())
                .Where(x => x.IsDeleted == false);
        }

        public async Task<IEnumerable<SaleForGettingDto>> GetSalesOfClientAsync(string clientId, int? page = 1, int? size = 10, string sortBy = "", bool ascending = true)
        {
            var resposne = await clientApi.GetSalesOfClientAsync(clientId, page, size, sortBy, ascending);

            if (!resposne.IsSuccess)
            {
                return Enumerable.Empty<SaleForGettingDto>();
            }

            return
                JsonConvert.DeserializeObject<IEnumerable<SaleForGettingDto>>(resposne.Result.ToString())
                .Where(x => x.IsDeleted == false);
        }

        public async Task<bool> DeleteItemOfClientAsync(string clientId, string itemId)
        {
            try
            {
                await clientApi.DeleteItemOfClientAsync(clientId, itemId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SellItemOfClientAsync(SellItemCommand sellItem)
        {
            try
            {
                await clientApi.SellItemAsync(sellItem);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddNewClientAsync(CreateClientCommand createClientModel)
        {
            try
            {
                await clientApi.AddNewClientAsync(createClientModel);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
