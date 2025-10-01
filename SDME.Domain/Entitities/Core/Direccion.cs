using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDME.Domain.Base;

namespace SDME.Domain.Entities.Core
{
    /// Direcciones de entrega del usuario: Direcciones para delivery
    public class Direccion : AuditEntity
    {
        public int UsuarioId { get; set; }
        public string NombreDireccion { get; set; } = string.Empty; // Ej: "Casa", "Oficina"
        public string Calle { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public bool EsPrincipal { get; set; } = false;

        // Relaciones
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

        // Métodos del dominio
        public string DireccionCompleta =>
            $"{Calle} #{Numero}, {Sector}, {Ciudad}";
    }
}
