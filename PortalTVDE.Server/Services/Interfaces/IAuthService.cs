using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    }
}
