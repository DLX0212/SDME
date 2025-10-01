using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDME.Domain.Base;
using SDME.Domain.Enums;
using SDME.Domain.Entities.Core;

namespace SDME.Domain.Entities.Pagos
{
    /// Información de pago del pedido Integración con pasarelas (Stripe, PayPal, Azul) y contra entrega

    public class Pago : AuditEntity
    {
        public int PedidoId { get; set; }
        public TipoMetodoPago MetodoPago { get; set; }
        public decimal Monto { get; set; }
        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;
        public string? TransaccionId { get; set; } // ID de la pasarela externa
        public string? ReferenciaPasarela { get; set; }
        public DateTime? FechaProcesamiento { get; set; }
        public string? MensajeError { get; set; }

        // Relaciones
        public virtual Core.Pedido Pedido { get; set; } = null!;

        // Métodos del dominio
        public void MarcarComoAprobado(string transaccionId)
        {
            Estado = EstadoPago.Aprobado;
            TransaccionId = transaccionId;
            FechaProcesamiento = DateTime.UtcNow;
        }

        public void MarcarComoRechazado(string mensajeError)
        {
            Estado = EstadoPago.Rechazado;
            MensajeError = mensajeError;
            FechaProcesamiento = DateTime.UtcNow;
        }

        public bool RequiereProcesamiento =>
            MetodoPago != TipoMetodoPago.ContraEntrega;
    }
}
