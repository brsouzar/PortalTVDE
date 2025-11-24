using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Client.Services.Interfaces
{
    public interface IMediatorClientService
    {
        Task<PaginatedResultDto<MediatorDto>> GetMediatorsAsync(MediatorQueryDto query);
        Task<MediatorDto> CreateMediatorAsync(MediatorCreateDto dto);
        Task<MediatorDto> UpdateMediatorAsync(MediatorUpdateDto dto);
    }
}
