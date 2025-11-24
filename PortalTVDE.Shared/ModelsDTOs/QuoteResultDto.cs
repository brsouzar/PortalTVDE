using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class QuoteResultDto
    {
        public decimal BasePremium { get; set; }
        public decimal AgeAdjustmentPercentage { get; set; } // Ex: 0.25
        public decimal AgeAdjustmentValue { get; set; } // Valor em €
        public decimal UsageSurchargeValue { get; set; } // Uso TVDE (+12%)
        public decimal CitySurchargeValue { get; set; } // Lisboa/Porto (+5%)
        public decimal NCBDiscountPercentage { get; set; } // Ex: -0.15
        public decimal NCBDiscountValue { get; set; } // Valor em €
        public decimal OptionalCoveragesTotal { get; set; } // GLASS + ROADSIDE
        public List<string> CoverageItems { get; set; } = new List<string>(); // Lista de coberturas ativas

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPremium { get; set; }
    }
}
