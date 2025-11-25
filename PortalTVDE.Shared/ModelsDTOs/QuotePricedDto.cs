using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class QuotePricedDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public QuoteBreakdownDto Breakdown { get; set; } = new QuoteBreakdownDto();

    }
}
