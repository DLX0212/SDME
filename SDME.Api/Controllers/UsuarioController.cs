// API/Controllers/UsuariosController.cs
using Microsoft.AspNetCore.Mvc;
using SDME.Application.DTOs.Usuario;
using SDME.Application.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace SDME.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            IUsuarioService usuarioService,
            ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        [HttpPost("registrar")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<UsuarioDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _usuarioService.RegistrarAsync(dto);

                if (!resultado.Exito)
                {
                    return BadRequest(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el endpoint de registro");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<LoginResponseDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _usuarioService.LoginAsync(dto);

                if (!resultado.Exito)
                {
                    return Unauthorized(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el endpoint de login");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtener usuario por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<UsuarioDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _usuarioService.ObtenerPorIdAsync(id);

                if (!resultado.Exito)
                {
                    return NotFound(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Actualizar información del usuario
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<UsuarioDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _usuarioService.ActualizarAsync(id, dto);

                if (!resultado.Exito)
                {
                    return NotFound(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {Id}", id);
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verificar si un email ya existe
        /// </summary>
        [HttpGet("verificar-email/{email}")]
        [ProducesResponseType(typeof(Application.DTOs.Common.ResponseDto<bool>), 200)]
        public async Task<IActionResult> VerificarEmail(string email)
        {
            try
            {
                var resultado = await _usuarioService.ExisteEmailAsync(email);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar email");
                return StatusCode(500, new { mensaje = "Error interno del servidor" });
            }
        }
    }
}