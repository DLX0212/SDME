using Microsoft.AspNetCore.Mvc;
using SDME.Application.Interfaces;
using SDME.Web.ViewModels;

namespace SDME.Web.Controllers
{
    /// Controlador para visualizar el menú de productos
    public class ProductosController : Controller 
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ICategoriaService categoriaService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _logger = logger;
        }
        // GET: /Productos o /menu: Muestra el menú completo con todas las categorías
        [HttpGet]
        public async Task<IActionResult> Index(int? categoriaId, string? busqueda)
        {
            try
            {
                var categoriasResult = await _categoriaService.ObtenerTodasAsync();
                var viewModel = new MenuViewModel
                {
                    Categorias = categoriasResult.Data ?? new(),
                    CategoriaSeleccionadaId = categoriaId,
                    TerminoBusqueda = busqueda
                };

                // Filtrar productos según parámetros
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    var resultadoBusqueda = await _productoService.BuscarAsync(busqueda);
                    viewModel.Productos = resultadoBusqueda.Data ?? new();
                    viewModel.TituloSeccion = $"Resultados para '{busqueda}'";
                }
                else if (categoriaId.HasValue)
                {
                    var resultadoCategoria = await _productoService.ObtenerPorCategoriaAsync(categoriaId.Value);
                    viewModel.Productos = resultadoCategoria.Data ?? new();

                    var categoriaNombre = viewModel.Categorias
                        .FirstOrDefault(c => c.Id == categoriaId.Value)?.Nombre ?? "Categoría";
                    viewModel.TituloSeccion = categoriaNombre;
                }
                else
                {
                    var todosProductos = await _productoService.ObtenerDisponiblesAsync();
                    viewModel.Productos = todosProductos.Data ?? new();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el menú");
                TempData["Error"] = "No se pudo cargar el menú. Por favor, intenta nuevamente.";
                return RedirectToAction("Index", "Home");
            }
        }
        /// GET: /Productos/Detalle/5: Muestra el detalle de un producto específico

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                var resultado = await _productoService.ObtenerPorIdAsync(id);

                if (!resultado.Exito || resultado.Data == null)
                {
                    TempData["Error"] = "Producto no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(resultado.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalle del producto {ProductoId}", id);
                TempData["Error"] = "No se pudo cargar el producto.";
                return RedirectToAction(nameof(Index));
            }
        }


        /// GET: /Productos/PorCategoria/1: Muestra productos de una categoría específica

        [HttpGet]
        public async Task<IActionResult> PorCategoria(int id)
        {
            return await Index(categoriaId: id, busqueda: null);
        }


        /// POST: /Productos/Buscar: Busca productos por término
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buscar(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { busqueda = termino });
        }
    }
}

    