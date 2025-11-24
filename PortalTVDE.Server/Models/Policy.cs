using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Server.Models
{
    public sealed class Policy : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string PolicyNumber { get; set; } = string.Empty; // Único

        [Required]
        public int QuoteId { get; set; }
        [ForeignKey(nameof(QuoteId))]
        public Quote? Quote { get; set; } // Relacionamento 1:1 com a cotação que a gerou

        public DateTime EffectiveFrom { get; set; } // Data de início de vigência
        public DateTime EffectiveTo { get; set; } // Data de fim de vigência

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPremium { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Commission { get; set; } // Comissão paga ao mediador

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
