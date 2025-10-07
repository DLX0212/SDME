using Microsoft.AspNetCore.Mvc;
using SDME.Application.DTOs.Categoria;
using SDME.Application.Interfaces;

namespace SDME.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(
            ICategoriaService categoriaService,
            ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las categorías con sus productos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<List<CategoriaDto>>), 200)]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var resultado = await _categoriaService.ObtenerTodasAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener categoría por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<CategoriaDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _categoriaService.ObtenerPorIdAsync(id);

                if (!resultado.Exito)
                {
                    return NotFound(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crear una nueva categoría (solo administradores)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<CategoriaDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Crear([FromBody] CrearCategoriaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _categoriaService.CrearAsync(dto);

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
                _logger.LogError(ex, "Error al crear categoría");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}
