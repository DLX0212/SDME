using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDME.Domain.Base;

namespace SDME.Domain.Entities.Core
{
    /// Detalle de cada línea del pedido: Detalles de productos en el carrito
    public class DetallePedido : Entity
    {
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? NotasEspeciales { get; set; } // Ej: "Sin cebolla"

        // Relaciones
        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;

        // Métodos del dominio
        public void CalcularSubtotal()
        {
            Subtotal = Cantidad * PrecioUnitario;
        }
    }
}
