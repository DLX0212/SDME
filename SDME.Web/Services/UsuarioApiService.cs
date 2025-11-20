using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Usuario;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SDME.Web.Services
{

    /// Servicio para consumir endpoints de Usuarios

    public class UsuarioApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsuarioApiService> _logger;
        private const string ENDPOINT = "Usuarios";

        public UsuarioApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<UsuarioApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("SDMEAPI");
            _logger = logger;
        }

        /// Registra un nuevo usuario

        public async Task<ResponseDto<UsuarioDto>> RegistrarAsync(RegistrarUsuarioDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{ENDPOINT}/registrar", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<UsuarioDto>>();
                    return resultado ?? ResponseDto<UsuarioDto>.Failure("Error al registrar usuario");
                }

                // Intentar leer mensaje de error específico
                try
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ResponseDto<UsuarioDto>>();
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Mensaje))
                    {
                        return errorResponse;
                    }
                }
                catch { }

                return ResponseDto<UsuarioDto>.Failure("Error al registrar usuario");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario");
                return ResponseDto<UsuarioDto>.Failure("Error de conexión con la API");
            }
        }

        /// Inicia sesión de usuario

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{ENDPOINT}/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<LoginResponseDto>>();
                    return resultado ?? ResponseDto<LoginResponseDto>.Failure("Error al iniciar sesión");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return ResponseDto<LoginResponseDto>.Failure("Email o contraseña incorrectos");
                }

                // Intentar leer mensaje de error
                try
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ResponseDto<LoginResponseDto>>();
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Mensaje))
                    {
                        return errorResponse;
                    }
                }
                catch { }

                return ResponseDto<LoginResponseDto>.Failure("Error al iniciar sesión");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión");
                return ResponseDto<LoginResponseDto>.Failure("Error de conexión con la API");
            }
        }

        /// Obtiene usuario por ID

        public async Task<ResponseDto<UsuarioDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<UsuarioDto>>();
                    return resultado ?? ResponseDto<UsuarioDto>.Failure("Usuario no encontrado");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<UsuarioDto>.Failure("Usuario no encontrado");
                }

                return ResponseDto<UsuarioDto>.Failure("Error al obtener usuario");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
                return ResponseDto<UsuarioDto>.Failure("Error de conexión con la API");
            }
        }

        /// Actualiza información del usuario

        public async Task<ResponseDto<UsuarioDto>> ActualizarAsync(int id, ActualizarUsuarioDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{ENDPOINT}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<UsuarioDto>>();
                    return resultado ?? ResponseDto<UsuarioDto>.Failure("Error al actualizar usuario");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<UsuarioDto>.Failure("Usuario no encontrado");
                }

                return ResponseDto<UsuarioDto>.Failure("Error al actualizar usuario");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UsuarioId}", id);
                return ResponseDto<UsuarioDto>.Failure("Error de conexión con la API");
            }
        }

        /// Verifica si un email ya existe

        public async Task<ResponseDto<bool>> ExisteEmailAsync(string email)
        {
            try
            {
                var encodedEmail = Uri.EscapeDataString(email);
                var response = await _httpClient.GetAsync($"{ENDPOINT}/verificar-email/{encodedEmail}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<bool>>();
                    return resultado ?? ResponseDto<bool>.Success(false);
                }

                return ResponseDto<bool>.Failure("Error al verificar email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar email {Email}", email);
                return ResponseDto<bool>.Failure("Error de conexión con la API");
            }
        }
    }
}
