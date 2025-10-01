using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDME.Domain.Base;

namespace SDME.Domain.Entities.Core
{
    /// Carrito de compras y pedidos temporal

    public class Carrito : AuditEntity
    {
        public int UsuarioId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        // Relaciones
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;

        // Métodos del dominio
        public decimal CalcularSubtotal()
        {
            return Cantidad * Producto.Precio;
        }

        public void ActualizarCantidad(int nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0");

            Cantidad = nuevaCantidad;
        }
    }
}
