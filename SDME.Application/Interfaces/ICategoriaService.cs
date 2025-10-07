using SDME.Application.DTOs.Categoria;
using SDME.Application.DTOs.Common;

namespace SDME.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<ResponseDto<List<CategoriaDto>>> ObtenerTodasAsync();
        Task<ResponseDto<CategoriaDto>> ObtenerPorIdAsync(int id);
        Task<ResponseDto<CategoriaDto>> CrearAsync(CrearCategoriaDto dto);
    }
}
