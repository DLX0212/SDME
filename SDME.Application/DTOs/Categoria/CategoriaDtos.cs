using SDME.Application.DTOs.Base;

namespace SDME.Application.DTOs.Categoria
{
    public class CategoriaDto : AuditDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public int Orden { get; set; }
        public int CantidadProductos { get; set; }
    }

    public class CrearCategoriaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public int Orden { get; set; }
    }

    public class ActualizarCategoriaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public int Orden { get; set; }
    }
}