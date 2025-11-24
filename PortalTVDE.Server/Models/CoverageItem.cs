using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Server.Models
{
    public sealed class CoverageItem : BaseEntity
    {
        [Required]
        public int QuoteId { get; set; }

        [ForeignKey(nameof(QuoteId))]
        public Quote? Quote { get; set; }

        [Required]
        [MaxLength(100)]
        public string CoverageCode { get; set; } = string.Empty; // TPL|GLASS|ROADSIDE
       
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Limit { get; set; } // Limite de cobertura (opcional)
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Deductible { get; set; } // Franquia (opcional)

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PremiumPart { get; set; } // Valor do prêmio para esta cobertura
    }
}
