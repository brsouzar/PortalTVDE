using PortalTVDE.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class MediatorDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public MediatorTier Tier { get; set; }
        public decimal CommissionRate { get; set; }
        public string? Email { get; set; }
    }
}
