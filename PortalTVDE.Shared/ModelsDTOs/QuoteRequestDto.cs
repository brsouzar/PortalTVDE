using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class QuoteRequestDto
    {
        // 1. Dados do Cliente
        [Required]
        public DateTime ClientBirthDate { get; set; }
        [Required]
        public int NCBYears { get; set; } // No Claims Bonus Years (Anos sem sinistro)
        [Required]
        public string ClientCity { get; set; } = string.Empty; // Para City Surcharge (Lisboa/Porto)

        // 2. Dados do Veículo
        [Required]
        public int VehiclePowerKW { get; set; }

        // 3. Coberturas Opcionais
        public bool IsGlassSelected { get; set; }
        public bool IsRoadsideSelected { get; set; }
    }
}
