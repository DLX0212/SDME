using SDME.Application.DTOs.Base;

namespace SDME.Application.DTOs.Producto
{
    public class ProductoDto : AuditDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public int Stock { get; set; }
        public bool Disponible { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
    }

    public class CrearProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
    }

    public class ActualizarProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public int Stock { get; set; }
        public bool Disponible { get; set; }
    }
}
