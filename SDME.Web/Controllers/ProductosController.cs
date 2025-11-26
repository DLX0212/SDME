using Microsoft.AspNetCore.Mvc;
using SDME.Web.Services.Interfaces;
using SDME.Web.ViewModels;

namespace SDME.Web.Controllers
{
    public class ProductosController : Controller
    {

        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            IProductoService productoService,
            ICategoriaService categoriaService,
            ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _categoriaService = categoriaService;
            _logger = logger;
        }

        /// GET: /Productos: Muestra el menú completo con todas las categorías

        [HttpGet]
        public async Task<IActionResult> Index(int? categoriaId, string? busqueda)
        {
            try
            {
   
                var categoriasResult = await _categoriaService.ObtenerTodosAsync();

                var viewModel = new MenuViewModel
                {
 
                    Categorias = categoriasResult.Exito ? categoriasResult.Data ?? new() : new(),
                    CategoriaSeleccionadaId = categoriaId,
                    TerminoBusqueda = busqueda
                };

                // Filtrar productos
                if (!string.IsNullOrWhiteSpace(busqueda))
                {

                    var resultadoBusqueda = await _productoService.BuscarAsync(busqueda);
                    viewModel.Productos = resultadoBusqueda.Exito ? resultadoBusqueda.Data ?? new() : new();
                    viewModel.TituloSeccion = $"Resultados para '{busqueda}'";

                    if (!resultadoBusqueda.Exito) TempData["Warning"] = resultadoBusqueda.Mensaje;
                }
                else if (categoriaId.HasValue)
                {
                    var resultadoCategoria = await _productoService.ObtenerPorCategoriaAsync(categoriaId.Value);
                    viewModel.Productos = resultadoCategoria.Exito ? resultadoCategoria.Data ?? new() : new();

                    var categoriaNombre = viewModel.Categorias
                        .FirstOrDefault(c => c.Id == categoriaId.Value)?.Nombre ?? "Categoría";
                    viewModel.TituloSeccion = categoriaNombre;

                    if (!resultadoCategoria.Exito) TempData["Warning"] = resultadoCategoria.Mensaje;
                }
                else
                {
                    var todosProductos = await _productoService.ObtenerDisponiblesAsync();
                    viewModel.Productos = todosProductos.Exito ? todosProductos.Data ?? new() : new();

                    if (!todosProductos.Exito) TempData["Warning"] = todosProductos.Mensaje;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el menú");
                TempData["Error"] = "No se pudo cargar el menú.";
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

