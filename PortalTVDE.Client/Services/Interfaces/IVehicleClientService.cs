using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Client.Services.Interfaces
{
    public interface IVehicleClientService
    {
        Task<PaginatedVehiclesResponse> GetVehiclesAsync(VehicleQueryDto query);

        Task<List<VehicleDto>> GetAllSimpleVehiclesAsync();
        Task<VehicleDto?> GetVehicleByIdAsync(int id);
        Task CreateVehicleAsync(VehicleCreateDto dto);
        Task UpdateVehicleAsync(int id, VehicleCreateDto dto);
        Task DeleteVehicleAsync(int id);
    }
}
