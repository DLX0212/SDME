using Microsoft.AspNetCore.Mvc;
using SDME.Web.ViewModels;
using SDME.Web.Services;

namespace SDME.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CategoriaApiService _categoriaApiService;
        private readonly ProductoApiService _productoApiService;

        public HomeController(
            ILogger<HomeController> logger,
            CategoriaApiService categoriaApiService,
            ProductoApiService productoApiService)
        {
            _logger = logger;
            _categoriaApiService = categoriaApiService;
            _productoApiService = productoApiService;
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
                var categoriasResult = await _categoriaApiService.ObtenerTodasAsync();
                if (categoriasResult.Exito)
                {
                    viewModel.Categorias = categoriasResult.Data ?? new();
                }

                // Obtener productos destacados (primeros 6 disponibles)
                var productosResult = await _productoApiService.ObtenerDisponiblesAsync();
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
