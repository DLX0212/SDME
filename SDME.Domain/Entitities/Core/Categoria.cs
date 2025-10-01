using SDME.Domain.Base;

namespace SDME.Domain.Entities.Core
{
    /// Categoría de productos (Empanadas, Bebidas, Combos)

    public class Categoria : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public int Orden { get; set; } // Para ordenar las categorías en el menú

        // Relaciones
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
