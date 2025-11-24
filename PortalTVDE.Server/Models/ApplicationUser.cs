using Microsoft.AspNetCore.Identity;

namespace PortalTVDE.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Exemplo: ligar o utilizador ao Mediator
        public ApplicationUser() : base()
        {
            // Opcional: inicializações podem ir aqui
        }
        public int? MediatorId { get; set; }

        public virtual Mediator? Mediator { get; set; }
    }
}
