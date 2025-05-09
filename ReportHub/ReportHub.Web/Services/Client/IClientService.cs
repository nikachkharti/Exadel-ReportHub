using ReportHub.Web.Models.Clients.DTOs;

namespace ReportHub.Web.Services.Client
{
    public interface IClientService
    {
        Task<IEnumerable<ClientForGettingDto>> GetClientsAsync(int? page = 1, int? size = 10, string sortBy = "", bool ascending = true);
    }
}
