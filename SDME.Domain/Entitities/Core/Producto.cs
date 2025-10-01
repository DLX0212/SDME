using SDME.Domain.Base;

namespace SDME.Domain.Entities.Core
{
    /// Exploración del menú con productos categorizados(Empanadas, bebidas, combos)
    public class Producto : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public int CategoriaId { get; set; }
        public int Stock { get; set; } // Inventario disponible
        public bool Disponible { get; set; } = true;

        // Relaciones
        public virtual Categoria Categoria { get; set; } = null!;
        public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();

        // Métodos del dominio
        public bool TieneStock(int cantidad)
        {
            return Stock >= cantidad && Disponible;
        }

        public void ReducirStock(int cantidad)
        {
            if (!TieneStock(cantidad))
                throw new InvalidOperationException($"Stock insuficiente para el producto {Nombre}");

            Stock -= cantidad;
        }

        public void AumentarStock(int cantidad)
        {
            Stock += cantidad;
        }
    }
}