// API/Controllers/ProductosController.cs
using Microsoft.AspNetCore.Mvc;
using SDME.Application.DTOs.Producto;
using SDME.Application.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace SDME.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            IProductoService productoService,
            ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los productos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<ProductoDto>>), 200)]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var resultado = await _productoService.ObtenerTodosAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener producto por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<ProductoDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _productoService.ObtenerPorIdAsync(id);

                if (!resultado.Exito)
                {
                    return NotFound(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener productos por categoría
        /// </summary>
        [HttpGet("categoria/{categoriaId}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<ProductoDto>>), 200)]
        public async Task<IActionResult> ObtenerPorCategoria(int categoriaId)
        {
            try
            {
                var resultado = await _productoService.ObtenerPorCategoriaAsync(categoriaId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos por categoría");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener productos disponibles
        /// </summary>
        [HttpGet("disponibles")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<ProductoDto>>), 200)]
        public async Task<IActionResult> ObtenerDisponibles()
        {
            try
            {
                var resultado = await _productoService.ObtenerDisponiblesAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos disponibles");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Buscar productos por término
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<ProductoDto>>), 200)]
        public async Task<IActionResult> Buscar([FromQuery] string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                {
                    return BadRequest(new { mensaje = "El término de búsqueda es requerido" });
                }

                var resultado = await _productoService.BuscarAsync(termino);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crear un nuevo producto (solo administradores)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<ProductoDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Crear([FromBody] CrearProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _productoService.CrearAsync(dto);

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
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualizar un producto (solo administradores)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<ProductoDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _productoService.ActualizarAsync(id, dto);

                if (!resultado.Exito)
                {
                    return NotFound(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Eliminar un producto (solo administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _productoService.EliminarAsync(id);

                if (!resultado.Exito)
                {
                    return NotFound(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}