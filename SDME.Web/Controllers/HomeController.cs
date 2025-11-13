using Microsoft.AspNetCore.Mvc;
using SDME.Application.Interfaces;
using SDME.Web.ViewModels;


namespace SDME.Web.Controllers
{
        /// <summary>
        /// Controlador para la página principal y páginas estáticas
        /// </summary>
        public class HomeController : Controller
        {
            private readonly ICategoriaService _categoriaService;
            private readonly IProductoService _productoService;
            private readonly ILogger<HomeController> _logger;

            public HomeController(
                ICategoriaService categoriaService,
                IProductoService productoService,
                ILogger<HomeController> logger)
            {
                _categoriaService = categoriaService;
                _productoService = productoService;
                _logger = logger;
            }

            /// <summary>
            /// Página principal - Muestra categorías y productos destacados
            /// </summary>
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                try
                {
                    var categoriasResult = await _categoriaService.ObtenerTodasAsync();
                    var productosResult = await _productoService.ObtenerDisponiblesAsync();

                    var viewModel = new HomeViewModel
                    {
                        Categorias = categoriasResult.Data ?? new(),
                        ProductosDestacados = productosResult.Data?.Take(6).ToList() ?? new(),
                        UsuarioAutenticado = User.Identity?.IsAuthenticated ?? false,
                        NombreUsuario = User.Identity?.Name
                    };

                    return View(viewModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al cargar la página principal");
                    return View("Error");
                }
            }

            /// <summary>
            /// Página Acerca de Nosotros
            /// </summary>
            [HttpGet]
            public IActionResult About()
            {
                ViewData["Title"] = "Acerca de Nosotros";
                return View();
            }

            /// <summary>
            /// Página de Contacto
            /// </summary>
            [HttpGet]
            public IActionResult Contact()
            {
                ViewData["Title"] = "Contacto";
                return View();
            }

            /// <summary>
            /// Página de Error genérica
            /// </summary>
            [HttpGet]
            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View();
            }
        }
    }
