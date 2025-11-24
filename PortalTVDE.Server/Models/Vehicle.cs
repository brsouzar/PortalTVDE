using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Server.Models
{
    public sealed class Vehicle : BaseEntity
    {
        [Required]
        [MaxLength(10)]
        public string LicensePlate { get; set; } = string.Empty; // Único

        [Required]
        public string Make { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        public int PowerKW { get; set; } // Potência em KiloWatts

        public int Year { get; set; } // Ano de fabrico

        [Required]
        public string Usage { get; set; } = "TVDE"; // Uso: TVDE
    }
}

