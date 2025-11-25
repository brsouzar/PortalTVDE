using Microsoft.AspNetCore.Identity;
using PortalTVDE.Server.Models;
using System.Threading.Tasks;

namespace PortalTVDE.Server.Services
{
    // Este serviço é Scoped, pois usa o UserManager, que é Scoped.
    public class UserSetupService{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserSetupService> _logger; // Adicionando ILogger

        public UserSetupService(UserManager<ApplicationUser> userManager, ILogger<UserSetupService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Corrige o PasswordHash do usuário no banco de dados, regerando-o com a senha correta.
        /// Este método deve ser executado UMA VEZ.
        /// </summary>
        /// <param name="email">O e-mail do usuário que precisa de correção.</param>
        /// <param name="newPassword">A senha em texto simples (ex: "Passw0rd").</param>
        /// <returns>Verdadeiro se a correção for bem-sucedida, Falso caso contrário.</returns>
        public async Task<bool> CorrectUserPasswordHashAsync(string email, string newPassword)
        {
            try
            {
                // 1. Encontra o usuário pelo e-mail
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("Correção de Hash: Usuário com e-mail {Email} não encontrado.", email);
                    return false;
                }

                // 2. Tenta remover qualquer senha existente (para garantir que a próxima seja o hash principal)
                var removeResult = await _userManager.RemovePasswordAsync(user);

                if (!removeResult.Succeeded && removeResult.Errors.Any(e => e.Code != "Passwordless"))
                {
                    _logger.LogWarning("Correção de Hash: Falha ao remover a senha antiga para {Email}.", email);
                }

                // 3. Adiciona a nova senha. ESTE PASSO GERA O HASH CORRETO.
                // O UserManager cuidará de gerar o hash com salt e salvar no banco.
                var addResult = await _userManager.AddPasswordAsync(user, newPassword);

                if (addResult.Succeeded)
                {
                    _logger.LogInformation("Correção de Hash: Hash de senha corrigido com sucesso para {Email}.", email);
                    return true;
                }
                else
                {
                    _logger.LogError("Correção de Hash: Falha ao aplicar a nova senha para {Email}. Erros: {Errors}",
                        email, string.Join(", ", addResult.Errors.Select(e => e.Description)));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Correção de Hash: Erro inesperado ao tentar corrigir o hash de senha.");
                return false;
            }
        }
    }
}