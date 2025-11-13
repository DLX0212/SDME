using SDME.Application.DTOs.Categoria;
using SDME.Application.DTOs.Producto;

namespace SDME.Web.ViewModels
{

    ///ViewModel para la página principal
    /// Muestra categorías destacadas y productos populares
    public class HomeViewModel
    {
        public List<CategoriaDto> Categorias { get; set; } = new();
        public List<ProductoDto> ProductosDestacados { get; set; } = new();
        public string MensajeBienvenida { get; set; } = "¡Bienvenido a D' Méndez Empanadas!";
        public bool UsuarioAutenticado { get; set; }
        public string? NombreUsuario { get; set; }
    }
}

