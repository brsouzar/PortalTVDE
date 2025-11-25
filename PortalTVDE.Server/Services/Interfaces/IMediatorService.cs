using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services.Interfaces
{
    public interface IMediatorService
    {
        Task<PaginatedResultDto<MediatorDto>> GetAllAsync(MediatorQueryDto query);
        Task<MediatorDto> CreateAsync(MediatorCreateDto dto);
        Task<MediatorDto> UpdateAsync(MediatorUpdateDto dto);
    }
}
