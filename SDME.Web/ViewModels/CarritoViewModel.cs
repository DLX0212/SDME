using SDME.Application.DTOs.Producto;

namespace SDME.Web.ViewModels
{
    /// ViewModel para el carrito de compras
    public class CarritoViewModel
    {
        public List<ItemCarritoViewModel> Items { get; set; } = new();
        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        public decimal Impuesto => Subtotal * 0.18m; // ITBIS 18%
        public decimal Total => Subtotal + Impuesto;
        public int CantidadTotal => Items.Sum(i => i.Cantidad);
        public bool TieneItems => Items.Any();
    }

    /// Representa un item individual en el carrito
    public class ItemCarritoViewModel
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => PrecioUnitario * Cantidad;
        public string? NotasEspeciales { get; set; }
        public int StockDisponible { get; set; }
    }

    /// ViewModel para agregar/actualizar items del carrito
    public class AgregarCarritoViewModel
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; } = 1;
        public string? NotasEspeciales { get; set; }
    }
}
