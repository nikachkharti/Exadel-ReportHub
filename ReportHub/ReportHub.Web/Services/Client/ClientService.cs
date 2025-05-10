using Newtonsoft.Json;
using ReportHub.Web.Models.Clients.DTOs;
using ReportHub.Web.Models.Items.DTOs;
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
            var resposne = await clientApi.GetItemsOfClientAsync(clientId);

            if (!resposne.IsSuccess)
            {
                return Enumerable.Empty<ItemForGettingDto>();
            }

            return JsonConvert.DeserializeObject<IEnumerable<ItemForGettingDto>>(resposne.Result.ToString());
        }
    }
}
