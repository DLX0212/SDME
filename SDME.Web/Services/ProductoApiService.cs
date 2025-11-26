using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Producto;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{

    public class ProductoApiService : BaseApiService<ProductoDto>, IProductoService
    {
        public ProductoApiService(IHttpClientService httpClient)
            : base(httpClient, "Productos")
        {
        }

        //Metodos específicos de ProductoApiService

        public async Task<ResponseDto<List<ProductoDto>>> BuscarAsync(string termino)
        {
            return await _httpClient.GetListAsync<ProductoDto>($"{_endpoint}/buscar/{termino}");
        }

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerDisponiblesAsync()
        {
            return await _httpClient.GetListAsync<ProductoDto>($"{_endpoint}/disponibles");
        }

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            return await _httpClient.GetListAsync<ProductoDto>($"{_endpoint}/categoria/{categoriaId}");
        }
    }
}

