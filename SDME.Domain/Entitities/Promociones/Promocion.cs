using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDME.Domain.Base;

namespace SDME.Domain.Entities.Promociones
{
    /// Promociones y descuentos Gestionar promociones (Panel Administrativo)
    public class Promocion : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string CodigoCupon { get; set; } = string.Empty;
        public decimal PorcentajeDescuento { get; set; }
        public decimal? MontoDescuento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? UsosMaximos { get; set; }
        public int UsosActuales { get; set; } = 0;
        public decimal? MontoMinimoCompra { get; set; }

        // Métodos del dominio
        public bool EstaVigente()
        {
            var ahora = DateTime.UtcNow;
            return ahora >= FechaInicio && ahora <= FechaFin && EstaActivo;
        }

        public bool PuedeUsarse()
        {
            if (!EstaVigente()) return false;
            if (UsosMaximos.HasValue && UsosActuales >= UsosMaximos.Value) return false;
            return true;
        }

        public decimal CalcularDescuento(decimal montoCompra)
        {
            if (!PuedeUsarse()) return 0;

            if (MontoMinimoCompra.HasValue && montoCompra < MontoMinimoCompra.Value)
                return 0;

            if (MontoDescuento.HasValue)
                return MontoDescuento.Value;

            return montoCompra * (PorcentajeDescuento / 100);
        }

        public void RegistrarUso()
        {
            UsosActuales++;
        }
    }
}