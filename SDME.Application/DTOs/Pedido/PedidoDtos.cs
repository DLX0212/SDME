using SDME.Application.DTOs.Base;
using System;
using System.Collections.Generic;

namespace SDME.Application.DTOs.Pedido
{
    public class PedidoDto : AuditDto
    {
        public string NumeroPedido { get; set; } = string.Empty;
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public string? NotasEspeciales { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public List<DetallePedidoDto> Detalles { get; set; } = new();
    }

    public class DetallePedidoDto
    {
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string? NotasEspeciales { get; set; }
    }

    public class CrearPedidoDto
    {
        public int UsuarioId { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public int? DireccionEntregaId { get; set; }
        public string? NotasEspeciales { get; set; }
        public List<CrearDetallePedidoDto> Detalles { get; set; } = new();
    }

    public class CrearDetallePedidoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? NotasEspeciales { get; set; }
    }

    public class ActualizarEstadoPedidoDto
    {
        public string NuevoEstado { get; set; } = string.Empty;
    }
}
