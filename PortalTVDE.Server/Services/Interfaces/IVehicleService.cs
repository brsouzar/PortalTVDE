using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<PaginatedVehiclesResponse> GetVehiclesAsync(VehicleQueryDto query);
        Task<List<VehicleDto>> GetAllSimpleVehiclesAsync();
        Task<VehicleDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(VehicleCreateDto dto);
        Task UpdateAsync(int id, VehicleCreateDto dto);
        Task DeleteAsync(int id);
    }
}
