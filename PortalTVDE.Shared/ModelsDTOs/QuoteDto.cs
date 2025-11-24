using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Shared.ModelsDTOs
{
    public class QuoteDto
    {

        [Required]
        public int Id { get; set; } // <<< Adicionado

        [Required]
        [MaxLength(50)]
        public string Number { get; set; } = string.Empty; // Número da cotação (Único)

        [Required]
        public int ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public ClientDto? Client { get; set; }

        [Required]
        public int MediatorId { get; set; }
        [ForeignKey(nameof(MediatorId))]
        public MediatorDto? Mediator { get; set; }

        [Required]
        public int VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public VehicleDto? Vehicle { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePremium { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Surcharges { get; set; } // Sobretaxas

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Discounts { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPremium { get; set; }

        [Required]
        public string Status { get; set; } = "Created"; // Created|Priced|Bound

        // Coleção de coberturas opcionais
        public ICollection<CoverageItemDto> CoverageItems { get; set; } = new List<CoverageItemDto>();

        public string ClientName { get; set; } = "";
    }
}
