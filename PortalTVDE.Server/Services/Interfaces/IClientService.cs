using PortalTVDE.Server.Models;
using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services.Interfaces
{
    public interface IClientService
    {
        Task<PaginatedClientsResponse> GetClientsAsync(ClientQueryDto query);
        Task<ClientDtoWithId?> GetByIdAsync(int id);
        Task<int> CreateAsync(ClientDto dto);
        Task UpdateAsync(int id, ClientDto dto);
        Task DeleteAsync(int id);
    }
}
