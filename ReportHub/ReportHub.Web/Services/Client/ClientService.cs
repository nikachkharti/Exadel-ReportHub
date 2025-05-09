using ReportHub.Web.Helper;
using ReportHub.Web.Models.Clients.DTOs;
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

            return response.Result.FromJson<IEnumerable<ClientForGettingDto>>();
        }
    }
}
