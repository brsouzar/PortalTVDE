using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalTVDE.Server.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; } // Quem criou (UserId)
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; } // Quem atualizou por último (UserId)
        public bool IsDeleted { get; set; } = false; // Soft delete

        // Concorrência Otimista
        [Timestamp]
        [ConcurrencyCheck]
        public byte[] RowVersion { get; set; } = default!;
    }
}
