using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Usuario;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{

    /// Servicio de usuarios que usa la abstracción IHttpClientService
    public class UsuarioApiService : IUsuarioService
    {
        private readonly IHttpClientService _httpClient;
        private readonly ILogger<UsuarioApiService> _logger;
        private const string ENDPOINT = "Usuarios";

        public UsuarioApiService(
            IHttpClientService httpClient,
            ILogger<UsuarioApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Métodos CRUD heredados de IApiService<UsuarioDto>
        public async Task<ResponseDto<List<UsuarioDto>>> ObtenerTodosAsync()
        {
            return await _httpClient.GetListAsync<UsuarioDto>(ENDPOINT);
        }

        public async Task<ResponseDto<UsuarioDto>> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetAsync<UsuarioDto>($"{ENDPOINT}/{id}");
        }

        public async Task<ResponseDto<UsuarioDto>> CrearAsync(object dto)
        {
            return await _httpClient.PostAsync<object, UsuarioDto>(ENDPOINT, dto);
        }

        public async Task<ResponseDto<UsuarioDto>> ActualizarAsync(int id, object dto)
        {
            return await _httpClient.PutAsync<object, UsuarioDto>($"{ENDPOINT}/{id}", dto);
        }

        public async Task<ResponseDto<bool>> EliminarAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{ENDPOINT}/{id}");
        }

        // Métodos específicos de IUsuarioService
        public async Task<ResponseDto<UsuarioDto>> RegistrarAsync(RegistrarUsuarioDto dto)
        {
            return await _httpClient.PostAsync<RegistrarUsuarioDto, UsuarioDto>(
                $"{ENDPOINT}/registrar",
                dto);
        }

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            return await _httpClient.PostAsync<LoginDto, LoginResponseDto>(
                $"{ENDPOINT}/login",
                dto);
        }

        public async Task<ResponseDto<bool>> ExisteEmailAsync(string email)
        {
            var encodedEmail = Uri.EscapeDataString(email);
            return await _httpClient.GetPrimitiveAsync<bool>($"{ENDPOINT}/verificar-email/{encodedEmail}");

        }
    }
}

