using SDME.Domain.Base;
using SDME.Domain.Enums;

namespace SDME.Domain.Entities.Core
{
    /// Entidad Usuario (Clientes y Administradores) Registro, Autenticación y gestión de usuarios
    public class Usuario : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }

        // Relaciones
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public virtual ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();

        // Métodos del dominio
        public string NombreCompleto => $"{Nombre} {Apellido}";

        public bool EsAdministrador => TipoUsuario == TipoUsuario.Administrador;

        public void ActualizarUltimoAcceso()
        {
            FechaUltimoAcceso = DateTime.UtcNow;
        }
    }
}