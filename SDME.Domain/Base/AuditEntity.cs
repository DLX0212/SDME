using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDME.Domain.Base
{
    public abstract class AuditEntity : Entity
    {
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }
        public string CreadoPor { get; set; } = string.Empty;
        public string? ModificadoPor { get; set; }
        public bool EstaActivo { get; set; } = true;
    }
}