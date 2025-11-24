using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class PaginatedVehiclesResponse
    {
        public int TotalCount { get; set; }
        public IEnumerable<VehicleDto> Items { get; set; } = new List<VehicleDto>();
    }
}
