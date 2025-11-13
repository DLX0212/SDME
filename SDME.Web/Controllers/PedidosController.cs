using Microsoft.AspNetCore.Mvc;
using SDME.Application.DTOs.Pedido;
using SDME.Application.Interfaces;
using SDME.Web.ViewModels;
using System.Text.Json;

namespace SDME.Web.Controllers
{
    /// Controlador para gestionar pedidos

    public class PedidosController : Controller
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;
        private const string CARRITO_SESSION_KEY = "CarritoCompras";

        public PedidosController(
            IPedidoService pedidoService,
            ILogger<PedidosController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }

        /// GET: /Pedidos: Lista de pedidos del usuario autenticado
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {

                int usuarioId = 1;

                var resultado = await _pedidoService.ObtenerPorUsuarioAsync(usuarioId);

                if (resultado.Exito && resultado.Data != null)
                {
                    return View(resultado.Data);
                }

                return View(new List<PedidoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del usuario");
                TempData["Error"] = "No se pudieron cargar tus pedidos.";
                return View(new List<PedidoDto>());
            }
        }

        /// GET: /Pedidos/Detalle/5: Muestra el detalle de un pedido específico
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {
                var resultado = await _pedidoService.ObtenerPorIdAsync(id);

                if (!resultado.Exito || resultado.Data == null)
                {
                    TempData["Error"] = "Pedido no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(resultado.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle del pedido {PedidoId}", id);
                TempData["Error"] = "No se pudo cargar el pedido.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// GET: /Pedidos/Checkout: Página para confirmar el pedido
        [HttpGet]
        public IActionResult Checkout()
        {
            var carritoJson = HttpContext.Session.GetString(CARRITO_SESSION_KEY);

            if (string.IsNullOrEmpty(carritoJson))
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }

            var carrito = JsonSerializer.Deserialize<CarritoViewModel>(carritoJson);

            if (carrito == null || !carrito.TieneItems)
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }

            return View(carrito);
        }

        /// POST: /Pedidos/Confirmar: Confirma y crea el pedido en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(CrearPedidoDto pedidoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos del pedido incompletos.";
                    return RedirectToAction(nameof(Checkout));
                }

                // Obtener carrito de sesión
                var carritoJson = HttpContext.Session.GetString(CARRITO_SESSION_KEY);
                if (string.IsNullOrEmpty(carritoJson))
                {
                    TempData["Error"] = "Tu carrito está vacío.";
                    return RedirectToAction("Index", "Carrito");
                }

                var carrito = JsonSerializer.Deserialize<CarritoViewModel>(carritoJson);
                if (carrito == null || !carrito.TieneItems)
                {
                    TempData["Error"] = "Tu carrito está vacío.";
                    return RedirectToAction("Index", "Carrito");
                }

                // Mapear carrito a detalles del pedido
                pedidoDto.Detalles = carrito.Items.Select(item => new CrearDetallePedidoDto
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    NotasEspeciales = item.NotasEspeciales
                }).ToList();

                // Crear el pedido
                var resultado = await _pedidoService.CrearPedidoAsync(pedidoDto);

                if (!resultado.Exito)
                {
                    TempData["Error"] = resultado.Mensaje ?? "No se pudo crear el pedido.";
                    return RedirectToAction(nameof(Checkout));
                }

                // Limpiar carrito
                HttpContext.Session.Remove(CARRITO_SESSION_KEY);

                TempData["Success"] = $"¡Pedido #{resultado.Data?.NumeroPedido} creado exitosamente!";
                return RedirectToAction(nameof(Detalle), new { id = resultado.Data?.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar pedido");
                TempData["Error"] = "Ocurrió un error al procesar tu pedido.";
                return RedirectToAction(nameof(Checkout));
            }
        }

        /// POST: /Pedidos/Cancelar/5: Cancela un pedido (solo si está en estado "Recibido")

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var actualizarDto = new ActualizarEstadoPedidoDto
                {
                    NuevoEstado = "Cancelado"
                };

                var resultado = await _pedidoService.ActualizarEstadoAsync(id, actualizarDto);

                if (resultado.Exito)
                {
                    TempData["Success"] = "Pedido cancelado exitosamente.";
                }
                else
                {
                    TempData["Error"] = resultado.Mensaje ?? "No se pudo cancelar el pedido.";
                }

                return RedirectToAction(nameof(Detalle), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar pedido {PedidoId}", id);
                TempData["Error"] = "No se pudo cancelar el pedido.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
        }
    }
}

