using Microsoft.AspNetCore.Mvc;
using SDME.Web.ViewModels;
using SDME.Web.Services;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICategoriaService _categoriaService;
        private readonly IProductoService _productoService;

        public HomeController(ILogger<HomeController> logger,ICategoriaService categoriaService, IProductoService productoService)   
        {
            _logger = logger;
            _categoriaService = categoriaService;
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                UsuarioAutenticado = HttpContext.Session.GetInt32("UsuarioId").HasValue,
                NombreUsuario = HttpContext.Session.GetString("UsuarioNombre")
            };

            try
            {
                // Obtener categorías
                // CORRECCIÓN 2: El método se llama ObtenerTodosAsync (no ObtenerTodasAsync)
                var categoriasResult = await _categoriaService.ObtenerTodosAsync();

                if (categoriasResult.Exito)
                {
                    viewModel.Categorias = categoriasResult.Data ?? new();
                }

                // Obtener productos destacados (primeros 6 disponibles)
                var productosResult = await _productoService.ObtenerDisponiblesAsync();

                if (productosResult.Exito && productosResult.Data != null)
                {
                    viewModel.ProductosDestacados = productosResult.Data.Take(6).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos del Home");
                // No mostrar error, solo devolver vista vacía
            }

            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
