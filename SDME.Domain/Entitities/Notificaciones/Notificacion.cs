using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDME.Domain.Base;
using SDME.Domain.Enums;
using SDME.Domain.Entities.Core;

namespace SDME.Domain.Entities.Notificaciones
{
    /// Notificaciones enviadas a usuarios: Notificaciones por correo y push del estado del pedido

    public class Notificacion : AuditEntity
    {
        public int UsuarioId { get; set; }
        public int? PedidoId { get; set; }
        public TipoNotificacion Tipo { get; set; }
        public string Asunto { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Enviada { get; set; } = false;
        public DateTime? FechaEnvio { get; set; }
        public bool Leida { get; set; } = false;
        public DateTime? FechaLectura { get; set; }

        // Relaciones
        public virtual Core.Usuario Usuario { get; set; } = null!;
        public virtual Core.Pedido? Pedido { get; set; }

        // Métodos del dominio
        public void MarcarComoEnviada()
        {
            Enviada = true;
            FechaEnvio = DateTime.UtcNow;
        }

        public void MarcarComoLeida()
        {
            Leida = true;
            FechaLectura = DateTime.UtcNow;
        }
    }
}
