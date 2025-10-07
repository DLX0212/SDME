using SDME.Application.DTOs.Producto;
using SDME.Application.DTOs.Common;

namespace SDME.Application.Interfaces
{
    public interface IProductoService
    {
        Task<ResponseDto<List<ProductoDto>>> ObtenerTodosAsync();
        Task<ResponseDto<ProductoDto>> ObtenerPorIdAsync(int id);
        Task<ResponseDto<List<ProductoDto>>> ObtenerPorCategoriaAsync(int categoriaId);
        Task<ResponseDto<List<ProductoDto>>> ObtenerDisponiblesAsync();
        Task<ResponseDto<ProductoDto>> CrearAsync(CrearProductoDto dto);
        Task<ResponseDto<ProductoDto>> ActualizarAsync(int id, ActualizarProductoDto dto);
        Task<ResponseDto<bool>> EliminarAsync(int id);
        Task<ResponseDto<List<ProductoDto>>> BuscarAsync(string termino);
    }
}
