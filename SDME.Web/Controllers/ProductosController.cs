using Microsoft.AspNetCore.Mvc;
using SDME.Web.Services;
using SDME.Web.ViewModels;

namespace SDME.Web.Controllers
{
    /// Controlador para visualizar el menú de productos  ( Ahora consume la API REST en lugar de servicios directos)
    public class ProductosController : Controller
    {
        private readonly ProductoApiService _productoApiService;
        private readonly CategoriaApiService _categoriaApiService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            ProductoApiService productoApiService,
            CategoriaApiService categoriaApiService,
            ILogger<ProductosController> logger)
        {
            _productoApiService = productoApiService;
            _categoriaApiService = categoriaApiService;
            _logger = logger;
        }

        /// GET: /Productos: Muestra el menú completo con todas las categorías
     
        [HttpGet]
        public async Task<IActionResult> Index(int? categoriaId, string? busqueda)
        {
            try
            {
                // Obtener categorías desde la API
                var categoriasResult = await _categoriaApiService.ObtenerTodasAsync();

                var viewModel = new MenuViewModel
                {
                    Categorias = categoriasResult.Exito ? categoriasResult.Data ?? new() : new(),
                    CategoriaSeleccionadaId = categoriaId,
                    TerminoBusqueda = busqueda
                };

                // Filtrar productos según parámetros
                if (!string.IsNullOrWhiteSpace(busqueda))
                {
                    // Buscar productos por término
                    var resultadoBusqueda = await _productoApiService.BuscarAsync(busqueda);
                    viewModel.Productos = resultadoBusqueda.Exito ? resultadoBusqueda.Data ?? new() : new();
                    viewModel.TituloSeccion = $"Resultados para '{busqueda}'";

                    if (!resultadoBusqueda.Exito)
                    {
                        TempData["Warning"] = resultadoBusqueda.Mensaje;
                    }
                }
                else if (categoriaId.HasValue)
                {
                    // Obtener productos por categoría
                    var resultadoCategoria = await _productoApiService.ObtenerPorCategoriaAsync(categoriaId.Value);
                    viewModel.Productos = resultadoCategoria.Exito ? resultadoCategoria.Data ?? new() : new();

                    var categoriaNombre = viewModel.Categorias
                        .FirstOrDefault(c => c.Id == categoriaId.Value)?.Nombre ?? "Categoría";
                    viewModel.TituloSeccion = categoriaNombre;

                    if (!resultadoCategoria.Exito)
                    {
                        TempData["Warning"] = resultadoCategoria.Mensaje;
                    }
                }
                else
                {
                    // Obtener todos los productos disponibles
                    var todosProductos = await _productoApiService.ObtenerDisponiblesAsync();
                    viewModel.Productos = todosProductos.Exito ? todosProductos.Data ?? new() : new();

                    if (!todosProductos.Exito)
                    {
                        TempData["Warning"] = todosProductos.Mensaje;
                    }
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
                var resultado = await _productoApiService.ObtenerPorIdAsync(id);

                if (!resultado.Exito || resultado.Data == null)
                {
                    TempData["Error"] = resultado.Mensaje ?? "Producto no encontrado.";
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

