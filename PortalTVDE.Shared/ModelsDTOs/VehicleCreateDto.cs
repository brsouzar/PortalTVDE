using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class VehicleCreateDto
    {
        [Required]
        [RegularExpression("^[A-Z0-9-]{6,10}$", ErrorMessage = "Formato inválido da matrícula.")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(80)]
        public string Make { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(80)]
        public string Model { get; set; } = string.Empty;

        [Range(20, 500)]
        public int PowerKW { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        [Required]
        [RegularExpression("^TVDE$", ErrorMessage = "Usage deve ser TVDE.")]
        public string Usage { get; set; } = "TVDE";
    }
}
