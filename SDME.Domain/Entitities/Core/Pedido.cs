using SDME.Domain.Base;
using SDME.Domain.Enums;
using SDME.Domain.Entities.Pagos;

namespace SDME.Domain.Entities.Core
{
    /// Pedido principal: Carrito de compras, confirmación de pedido y seguimiento
    public class Pedido : AuditEntity
    {
        public int UsuarioId { get; set; }
        public string NumeroPedido { get; set; } = string.Empty; // Ej: "PED-2025-0001"
        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;
        public EstadoPedido Estado { get; set; } = EstadoPedido.Recibido;
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string? NotasEspeciales { get; set; }
        public TipoEntrega TipoEntrega { get; set; }
        public int? DireccionEntregaId { get; set; }

        // Relaciones
        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Direccion? DireccionEntrega { get; set; }
        public virtual ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
        public virtual Pago? Pago { get; set; }

        // Métodos del dominio
        public void CalcularTotal()
        {
            Subtotal = DetallesPedido.Sum(d => d.Subtotal);
            Impuesto = Subtotal * 0.18m; // ITBIS 18% República Dominicana
            Total = Subtotal + Impuesto;
        }

        public void CambiarEstado(EstadoPedido nuevoEstado)
        {
            // Validaciones de transición de estado
            if (Estado == EstadoPedido.Entregado)
                throw new InvalidOperationException("No se puede cambiar el estado de un pedido ya entregado");

            if (Estado == EstadoPedido.Cancelado)
                throw new InvalidOperationException("No se puede cambiar el estado de un pedido cancelado");

            Estado = nuevoEstado;
            FechaModificacion = DateTime.UtcNow;
        }

        public void AgregarDetalle(DetallePedido detalle)
        {
            if (!detalle.Producto.TieneStock(detalle.Cantidad))
                throw new InvalidOperationException($"Stock insuficiente para {detalle.Producto.Nombre}");

            DetallesPedido.Add(detalle);
            CalcularTotal();
        }

        public void GenerarNumeroPedido()
        {
            NumeroPedido = $"PED-{DateTime.UtcNow:yyyy}-{Id:D6}";
        }
    }
}