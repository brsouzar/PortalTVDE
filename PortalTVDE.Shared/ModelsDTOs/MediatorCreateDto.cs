using PortalTVDE.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class MediatorCreateDto
    {
        [Required, StringLength(120, MinimumLength = 3)]
        public string? Name { get; set; }

        [Required]
        public MediatorTier Tier { get; set; }

        [Range(0.01, 0.50)]
        public decimal CommissionRate { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }
    }
}
