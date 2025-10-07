// API/Controllers/PedidosController.cs
using Microsoft.AspNetCore.Mvc;
using SDME.Application.DTOs.Pedido;
using SDME.Application.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace SDME.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(
            IPedidoService pedidoService,
            ILogger<PedidosController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }

        /// <summary>
        /// Crear un nuevo pedido
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<PedidoDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Crear([FromBody] CrearPedidoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _pedidoService.CrearPedidoAsync(dto);

                if (!resultado.Exito)
                {
                    return BadRequest(resultado);
                }

                return CreatedAtAction(
                    nameof(ObtenerPorId),
                    new { id = resultado.Data!.Id },
                    resultado
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener pedido por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<PedidoDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _pedidoService.ObtenerPorIdAsync(id);

                if (!resultado.Exito)
                {
                    return NotFound(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedido {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener pedidos por usuario
        /// </summary>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<PedidoDto>>), 200)]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            try
            {
                var resultado = await _pedidoService.ObtenerPorUsuarioAsync(usuarioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del usuario {UsuarioId}", usuarioId);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener pedidos por estado
        /// </summary>
        [HttpGet("estado/{estado}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<PedidoDto>>), 200)]
        public async Task<IActionResult> ObtenerPorEstado(string estado)
        {
            try
            {
                var resultado = await _pedidoService.ObtenerPorEstadoAsync(estado);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos por estado");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener pedidos del día actual
        /// </summary>
        [HttpGet("hoy")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<PedidoDto>>), 200)]
        public async Task<IActionResult> ObtenerPedidosDelDia()
        {
            try
            {
                var resultado = await _pedidoService.ObtenerPedidosDelDiaAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del día");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualizar estado del pedido (solo administradores)
        /// </summary>
        [HttpPatch("{id}/estado")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<PedidoDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoPedidoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _pedidoService.ActualizarEstadoAsync(id, dto);

                if (!resultado.Exito)
                {
                    return BadRequest(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
