using PortalTVDE.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class MediatorQueryDto
    {
        public string? Name { get; set; }          // Filtro por nome parcial
        public MediatorTier? Tier { get; set; }    // Filtro por Tier
        public int Page { get; set; } = 1;         // Página atual
        public int PageSize { get; set; } = 10;
    }
}
