using PortalTVDE.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Server.Models
{
    public class Mediator : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public MediatorTier Tier { get; set; }

        
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, 0.50)]
        public decimal CommissionRate { get; set; } // Percentual de comissão

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty; // Único

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
