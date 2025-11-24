using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class PolicyDto 
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public decimal TotalPremium { get; set; }
        public decimal Commission { get; set; }
    }
}
