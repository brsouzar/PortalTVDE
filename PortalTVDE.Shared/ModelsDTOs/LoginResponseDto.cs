using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    /// <summary>
    /// DTO para a resposta de Login bem-sucedido (contém o JWT).
    /// </summary>
    public class LoginResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
        // Opcional: Se o usuário for um mediador, incluir o ID do mediador.
        public int? MediatorId { get; set; }
    }
}
