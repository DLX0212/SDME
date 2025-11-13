using SDME.Application.DTOs.Categoria;
using SDME.Application.DTOs.Producto;

namespace SDME.Web.ViewModels
{
    /// ViewModel para la página del menú completo
    /// Permite filtrar por categoría y buscar productos
    public class MenuViewModel
    {
        public List<CategoriaDto> Categorias { get; set; } = new();
        public List<ProductoDto> Productos { get; set; } = new();
        public int? CategoriaSeleccionadaId { get; set; }
        public string? TerminoBusqueda { get; set; }
        public string TituloSeccion { get; set; } = "Nuestro Menú";
    }
}
