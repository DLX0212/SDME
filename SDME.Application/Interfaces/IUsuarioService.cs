using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Usuario;

namespace SDME.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<ResponseDto<UsuarioDto>> RegistrarAsync(RegistrarUsuarioDto dto);
        Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginDto dto);
        Task<ResponseDto<UsuarioDto>> ObtenerPorIdAsync(int id);
        Task<ResponseDto<UsuarioDto>> ActualizarAsync(int id, ActualizarUsuarioDto dto);
        Task<ResponseDto<bool>> ExisteEmailAsync(string email);
    }
}
