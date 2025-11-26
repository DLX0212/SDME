using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Producto;

namespace SDME.Web.Services.Interfaces
{

    /// Interfaz específica para servicio de Productos que usa IApiService con los metodos comunes (Refactory)

    public interface IProductoService : IApiService<ProductoDto>
    {
        Task<ResponseDto<List<ProductoDto>>> ObtenerPorCategoriaAsync(int categoriaId);
        Task<ResponseDto<List<ProductoDto>>> ObtenerDisponiblesAsync();
        Task<ResponseDto<List<ProductoDto>>> BuscarAsync(string termino);
    }
}