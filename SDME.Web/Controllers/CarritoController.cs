using Microsoft.AspNetCore.Mvc;
using SDME.Web.ViewModels;
using System.Text.Json;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Controllers
{
    /// Controlador para gestionar el carrito de compras
    /// Usa sesiones para almacenar el carrito temporalmente
    public class CarritoController : Controller
    {

        private readonly IProductoService _productoService;
        private readonly ILogger<CarritoController> _logger;
        private const string CARRITO_SESSION_KEY = "CarritoCompras";

  
        public CarritoController(
            IProductoService productoService,
            ILogger<CarritoController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }


        /// GET: /Carrito: Muestra el contenido del carrito
        [HttpGet]
        public IActionResult Index()
        {
            var carrito = ObtenerCarritoDeSesion();
            return View(carrito);
        }

        /// POST: /Carrito/Agregar: Agrega un producto al carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(int productoId, int cantidad = 1, string? notasEspeciales = null)
        {
            try
            {
                // Validar que el producto existe y tiene stock
                var productoResult = await _productoService.ObtenerPorIdAsync(productoId);

                if (!productoResult.Exito || productoResult.Data == null)
                {
                    TempData["Error"] = "Producto no encontrado.";
                    return RedirectToAction("Index", "Productos");
                }

                var producto = productoResult.Data;

                if (!producto.Disponible || producto.Stock < cantidad)
                {
                    TempData["Error"] = "Producto sin stock suficiente.";
                    return RedirectToAction("Index", "Productos");
                }

                // Obtener carrito de sesión
                var carrito = ObtenerCarritoDeSesion();

                // Verificar si el producto ya está en el carrito
                var itemExistente = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

                if (itemExistente != null)
                {
                    // Actualizar cantidad
                    itemExistente.Cantidad += cantidad;

                    // Validar que no exceda el stock
                    if (itemExistente.Cantidad > producto.Stock)
                    {
                        itemExistente.Cantidad = producto.Stock;
                        TempData["Warning"] = $"Solo hay {producto.Stock} unidades disponibles.";
                    }
                }
                else
                {
                    // Agregar nuevo item
                    carrito.Items.Add(new ItemCarritoViewModel
                    {
                        ProductoId = producto.Id,
                        Nombre = producto.Nombre,
                        ImagenUrl = producto.ImagenUrl,
                        PrecioUnitario = producto.Precio,
                        Cantidad = cantidad,
                        NotasEspeciales = notasEspeciales,
                        StockDisponible = producto.Stock
                    });
                }

                // Guardar en sesión
                GuardarCarritoEnSesion(carrito);

                TempData["Success"] = $"{producto.Nombre} agregado al carrito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto {ProductoId} al carrito", productoId);
                TempData["Error"] = "No se pudo agregar el producto al carrito.";
                return RedirectToAction("Index", "Productos");
            }
        }


        /// POST: /Carrito/Actualizar: Actualiza la cantidad de un item en el carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(int productoId, int cantidad)
        {
            try
            {
                if (cantidad <= 0)
                {
                    return await Eliminar(productoId);
                }

                var carrito = ObtenerCarritoDeSesion();
                var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

                if (item == null)
                {
                    TempData["Error"] = "Producto no encontrado en el carrito.";
                    return RedirectToAction(nameof(Index));
                }

                // Validar stock disponible
                var productoResult = await _productoService.ObtenerPorIdAsync(productoId);
                if (productoResult.Exito && productoResult.Data != null)
                {
                    if (cantidad > productoResult.Data.Stock)
                    {
                        cantidad = productoResult.Data.Stock;
                        TempData["Warning"] = $"Solo hay {productoResult.Data.Stock} unidades disponibles.";
                    }
                }

                item.Cantidad = cantidad;
                GuardarCarritoEnSesion(carrito);

                TempData["Success"] = "Carrito actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {ProductoId} en carrito", productoId);
                TempData["Error"] = "No se pudo actualizar el carrito.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// POST: /Carrito/Eliminar:Elimina un producto del carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int productoId)
        {
            try
            {
                var carrito = ObtenerCarritoDeSesion();
                var item = carrito.Items.FirstOrDefault(i => i.ProductoId == productoId);

                if (item != null)
                {
                    carrito.Items.Remove(item);
                    GuardarCarritoEnSesion(carrito);
                    TempData["Success"] = $"{item.Nombre} eliminado del carrito.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {ProductoId} del carrito", productoId);
                TempData["Error"] = "No se pudo eliminar el producto.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// POST: /Carrito/Vaciar:Vacía completamente el carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove(CARRITO_SESSION_KEY);
            TempData["Success"] = "Carrito vaciado.";
            return RedirectToAction(nameof(Index));
        }

        /// GET: /Carrito/CantidadItems:Devuelve la cantidad total de items (para mostrar en el navbar)
     
        [HttpGet]
        public IActionResult CantidadItems()
        {
            var carrito = ObtenerCarritoDeSesion();
            return Json(new { cantidad = carrito.CantidadTotal });
        }

        #region Métodos Privados - Gestión de Sesión

        private CarritoViewModel ObtenerCarritoDeSesion()
        {
            var carritoJson = HttpContext.Session.GetString(CARRITO_SESSION_KEY);

            if (string.IsNullOrEmpty(carritoJson))
            {
                return new CarritoViewModel();
            }

            try
            {
                return JsonSerializer.Deserialize<CarritoViewModel>(carritoJson) ?? new CarritoViewModel();
            }
            catch
            {
                return new CarritoViewModel();
            }
        }

        private void GuardarCarritoEnSesion(CarritoViewModel carrito)
        {
            var carritoJson = JsonSerializer.Serialize(carrito);
            HttpContext.Session.SetString(CARRITO_SESSION_KEY, carritoJson);
        }

        #endregion
    }
}

