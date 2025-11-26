using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Producto;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{
    /// <summary>
    /// Servicio de productos - USA la abstracción IHttpClientService
    /// Ahora es MUCHO más simple, sin código repetido
    /// </summary>
    public class ProductoApiService : IProductoService
    {
        private readonly IHttpClientService _httpClient;
        private readonly ILogger<ProductoApiService> _logger;
        private const string ENDPOINT = "Productos";

        public ProductoApiService(
            IHttpClientService httpClient,
            ILogger<ProductoApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Métodos CRUD heredados de IApiService<ProductoDto>
        public async Task<ResponseDto<List<ProductoDto>>> ObtenerTodosAsync()
        {
            return await _httpClient.GetListAsync<ProductoDto>(ENDPOINT);
        }

        public async Task<ResponseDto<ProductoDto>> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetAsync<ProductoDto>($"{ENDPOINT}/{id}");
        }

        public async Task<ResponseDto<ProductoDto>> CrearAsync(object dto)
        {
            return await _httpClient.PostAsync<object, ProductoDto>(ENDPOINT, dto);
        }

        public async Task<ResponseDto<ProductoDto>> ActualizarAsync(int id, object dto)
        {
            return await _httpClient.PutAsync<object, ProductoDto>($"{ENDPOINT}/{id}", dto);
        }

        public async Task<ResponseDto<bool>> EliminarAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{ENDPOINT}/{id}");
        }

        // ✅ Métodos específicos de IProductoService
        public async Task<ResponseDto<List<ProductoDto>>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            return await _httpClient.GetListAsync<ProductoDto>($"{ENDPOINT}/categoria/{categoriaId}");
        }

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerDisponiblesAsync()
        {
            return await _httpClient.GetListAsync<ProductoDto>($"{ENDPOINT}/disponibles");
        }

        public async Task<ResponseDto<List<ProductoDto>>> BuscarAsync(string termino)
        {
            return await _httpClient.GetListAsync<ProductoDto>(
                $"{ENDPOINT}/buscar?termino={Uri.EscapeDataString(termino)}");
        }
    }
}

