using System;
// DTO base para todas las entidades con información común
namespace SDME.Application.DTOs.Base
{
    public abstract class AuditDto : BaseDto
    {
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool EstaActivo { get; set; }
    }
}