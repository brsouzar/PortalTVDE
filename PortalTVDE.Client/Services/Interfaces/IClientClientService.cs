using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Client.Services.Interfaces
{
    public interface IClientClientService
    {
        Task<PaginatedClientsResponse> GetClientsAsync(ClientQueryDto query);
        Task<ClientDtoWithId?> GetClientByIdAsync(int id);
        Task CreateClientAsync(ClientDto dto);
        Task UpdateClientAsync(int id, ClientDto dto);
        Task DeleteClientAsync(int id);
    }
}
