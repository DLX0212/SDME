using Microsoft.AspNetCore.Mvc;
using SDME.Application.DTOs.Usuario;
using SDME.Web.Services;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Controllers
{

    /// Controlador para autenticación y gestión de cuenta, Ahora consume la API REST

    public class CuentaController : Controller
    {
        private readonly IUsuarioService _usuarioApiService;

        private readonly ILogger<CuentaController> _logger;
        public CuentaController(
        IUsuarioService usuarioService, ILogger<CuentaController> logger)
        {
            _usuarioApiService = usuarioService;
            _logger = logger;
        }


        /// GET: /Cuenta/Login: Muestra el formulario de inicio de sesión

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }


        /// POST: /Cuenta/Login: Procesa el inicio de sesión consumiendo la API

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(loginDto);
                }

                // Consumir API de login
                var resultado = await _usuarioApiService.LoginAsync(loginDto);

                if (!resultado.Exito || resultado.Data == null)
                {
                    ModelState.AddModelError(string.Empty, resultado.Mensaje ?? "Email o contraseña incorrectos.");
                    return View(loginDto);
                }

                // Guardar información en sesión
                HttpContext.Session.SetInt32("UsuarioId", resultado.Data.Usuario.Id);
                HttpContext.Session.SetString("UsuarioNombre", resultado.Data.Usuario.NombreCompleto);
                HttpContext.Session.SetString("UsuarioEmail", resultado.Data.Usuario.Email);
                HttpContext.Session.SetString("UsuarioToken", resultado.Data.Token);

                TempData["Success"] = $"¡Bienvenido, {resultado.Data.Usuario.Nombre}!";

                // Redirigir a la URL de retorno o al home
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login para {Email}", loginDto.Email);
                ModelState.AddModelError(string.Empty, "Ocurrió un error al iniciar sesión. Intenta nuevamente.");
                return View(loginDto);
            }
        }


        /// GET: /Cuenta/Registro: Muestra el formulario de registro

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }


        /// POST: /Cuenta/Registro: Procesa el registro de nuevo usuario consumiendo la API

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistrarUsuarioDto registroDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(registroDto);
                }

                // Verificar que el email no exista (consumir API)
                var emailExisteResult = await _usuarioApiService.ExisteEmailAsync(registroDto.Email);
                if (emailExisteResult.Exito && emailExisteResult.Data)
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado.");
                    return View(registroDto);
                }

                // Registrar usuario (consumir API)
                var resultado = await _usuarioApiService.RegistrarAsync(registroDto);

                if (!resultado.Exito)
                {
                    ModelState.AddModelError(string.Empty, resultado.Mensaje ?? "Error al registrar usuario.");
                    return View(registroDto);
                }

                TempData["Success"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro para {Email}", registroDto.Email);
                ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar tu cuenta. Intenta nuevamente.");
                return View(registroDto);
            }
        }


        /// POST: /Cuenta/Logout: Cierra la sesión del usuario

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Sesión cerrada exitosamente.";
            return RedirectToAction("Index", "Home");
        }


        /// GET: /Cuenta/Perfil: Muestra el perfil del usuario autenticado

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (!usuarioId.HasValue)
                {
                    TempData["Error"] = "Debes iniciar sesión para ver tu perfil.";
                    return RedirectToAction(nameof(Login));
                }

                // Consumir API para obtener usuario
                var resultado = await _usuarioApiService.ObtenerPorIdAsync(usuarioId.Value);

                if (!resultado.Exito || resultado.Data == null)
                {
                    TempData["Error"] = resultado.Mensaje ?? "No se pudo cargar tu perfil.";
                    return RedirectToAction("Index", "Home");
                }

                return View(resultado.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar perfil de usuario");
                TempData["Error"] = "No se pudo cargar tu perfil.";
                return RedirectToAction("Index", "Home");
            }
        }


        /// POST: /Cuenta/ActualizarPerfil: Actualiza la información del perfil del usuario

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPerfil(ActualizarUsuarioDto actualizarDto)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (!usuarioId.HasValue)
                {
                    TempData["Error"] = "Debes iniciar sesión.";
                    return RedirectToAction(nameof(Login));
                }

                if (!ModelState.IsValid)
                {
                    return View("Perfil", actualizarDto);
                }

                // Consumir API para actualizar
                var resultado = await _usuarioApiService.ActualizarAsync(usuarioId.Value, actualizarDto);

                if (resultado.Exito && resultado.Data != null)
                {
                    // Actualizar nombre en sesión
                    HttpContext.Session.SetString("UsuarioNombre", resultado.Data.NombreCompleto);
                    TempData["Success"] = "Perfil actualizado exitosamente.";
                }
                else
                {
                    TempData["Error"] = resultado.Mensaje ?? "No se pudo actualizar el perfil.";
                }

                return RedirectToAction(nameof(Perfil));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar perfil");
                TempData["Error"] = "No se pudo actualizar tu perfil.";
                return RedirectToAction(nameof(Perfil));
            }
        }


        /// GET: /Cuenta/VerificarEmail: Verifica si un email ya está registrado

        [HttpGet]
        public async Task<IActionResult> VerificarEmail(string email)
        {
            var resultado = await _usuarioApiService.ExisteEmailAsync(email);
            return Json(new { existe = resultado.Exito && resultado.Data });
        }
    }
}
