using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Usuario;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{
    public class UsuarioApiService : BaseApiService<UsuarioDto>, IUsuarioService
    {
        // Pasamos el IHttpClientService y el nombre del endpoint ("Usuarios") al padre
        public UsuarioApiService(IHttpClientService httpClient)
            : base(httpClient, "Usuarios")
        {
        }

        // Metodos específicos de UsuarioApiService

        public async Task<ResponseDto<UsuarioDto>> RegistrarAsync(RegistrarUsuarioDto dto)
        {
            return await _httpClient.PostAsync<RegistrarUsuarioDto, UsuarioDto>(
                $"{_endpoint}/registrar",
                dto);
        }

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            return await _httpClient.PostAsync<LoginDto, LoginResponseDto>(
                $"{_endpoint}/login",
                dto);
        }

        public async Task<ResponseDto<bool>> ExisteEmailAsync(string email)
        {
            var encodedEmail = Uri.EscapeDataString(email);
            // Usamos _httpClient y _endpoint que vienen de la clase Base
            return await _httpClient.GetPrimitiveAsync<bool>($"{_endpoint}/verificar-email/{encodedEmail}");
        }
    }
}

