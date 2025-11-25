using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class QuoteBreakdownDto
    {
        public decimal BasePremium { get; set; }
        public decimal AgeAdjustment { get; set; }
        public decimal UsageAdjustment { get; set; }
        public decimal CitySurcharge { get; set; }
        public decimal NcbDiscount { get; set; }
        public decimal OptionalCoverages { get; set; }
        public decimal Total { get; set; }
        public List<string> CoverageItems { get; set; } = new();
    }
}
