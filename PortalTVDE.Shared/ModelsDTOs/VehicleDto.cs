using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        public string Make { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int PowerKW { get; set; }

        public int Year { get; set; }

        public string Usage { get; set; } = "TVDE";
    }
}
