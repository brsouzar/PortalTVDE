using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class QuotePriceRequestDto
    {
        [Required] public int ClientId { get; set; }
        [Required] public int VehicleId { get; set; }
        [Required, StringLength(80, MinimumLength = 2)] public string City { get; set; } = string.Empty;
        [Range(0, 50)] public int NcbYears { get; set; }
        public bool IncludeGlass { get; set; }
        public bool IncludeRoadside { get; set; }
        [Required] public DateTime EffectiveFrom { get; set; }
        public int? MediatorId { get; set; }
    }
}
