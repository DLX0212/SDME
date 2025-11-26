using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Usuario;

namespace SDME.Web.Services.Interfaces
{

    /// Interfaz para servicio de Usuarios
    /// Hereda operaciones CRUD + métodos de autenticación

    public interface IUsuarioService : IApiService<UsuarioDto>
    {
        // Métodos de autenticación
        Task<ResponseDto<UsuarioDto>> RegistrarAsync(RegistrarUsuarioDto dto);
        Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto);
        Task<ResponseDto<bool>> ExisteEmailAsync(string email);
    }
}