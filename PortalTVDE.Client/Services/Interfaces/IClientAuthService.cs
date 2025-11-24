using PortalTVDE.Shared.ModelsDTOs;

namespace PortalTVDE.Client.Services.Interfaces
{
    public interface IClientAuthService
    {
        /// <summary>
        /// Tenta autenticar o usuário na API e retorna o token.
        /// </summary>
        Task<LoginResult> Login(LoginRequestDto loginRequest);

        /// <summary>
        /// Remove o token armazenado localmente e notifica a mudança de estado.
        /// </summary>
        Task Logout();

        Task<LoginResult> Register(RegisterRequestDto registerRequest);
    }
}
