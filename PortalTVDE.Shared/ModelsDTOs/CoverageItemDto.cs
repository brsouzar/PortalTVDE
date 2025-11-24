using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class CoverageItemDto
    {
        [Required]
        public int QuoteId { get; set; }

        [ForeignKey(nameof(QuoteId))]
        public QuoteDto? Quote { get; set; }

        [Required]
        public string CoverageCode { get; set; } = string.Empty; // TPL|GLASS|ROADSIDE

        public decimal? Limit { get; set; } // Limite de cobertura (opcional)
        public decimal? Deductible { get; set; } // Franquia (opcional)

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PremiumPart { get; set; } // Valor do prêmio para esta cobertura
    }
}
