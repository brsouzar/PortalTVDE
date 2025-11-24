using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PortalTVDE.Server.Models
{
    public sealed class Quote : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Number { get; set; } = string.Empty; // Número da cotação (Único)

        [Required]
        public int ClientId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public Clientt? Client { get; set; }

        public int? MediatorId { get; set; }
        [ForeignKey(nameof(MediatorId))]
        public Mediator? Mediator { get; set; }

        [Required]
        public int VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public Vehicle? Vehicle { get; set; }

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
        [MaxLength(20)]
        public string Status { get; set; } = "Created"; // Created|Priced|Bound

        // Coleção de coberturas opcionais
        [JsonIgnore]
        public ICollection<CoverageItem> CoverageItems { get; set; } = new List<CoverageItem>();
    }
}
