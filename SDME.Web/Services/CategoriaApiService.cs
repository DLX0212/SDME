using SDME.Application.DTOs.Categoria;
using SDME.Application.DTOs.Common;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{
    /// <summary>
    /// Servicio de categorías - USA la abstracción IHttpClientService
    /// </summary>
    public class CategoriaApiService : ICategoriaService
    {
        private readonly IHttpClientService _httpClient;
        private readonly ILogger<CategoriaApiService> _logger;
        private const string ENDPOINT = "Categorias";

        public CategoriaApiService(
            IHttpClientService httpClient,
            ILogger<CategoriaApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // ✅ Métodos CRUD heredados de IApiService<CategoriaDto>
        public async Task<ResponseDto<List<CategoriaDto>>> ObtenerTodosAsync()
        {
            return await _httpClient.GetListAsync<CategoriaDto>(ENDPOINT);
        }

        public async Task<ResponseDto<CategoriaDto>> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetAsync<CategoriaDto>($"{ENDPOINT}/{id}");
        }

        public async Task<ResponseDto<CategoriaDto>> CrearAsync(object dto)
        {
            return await _httpClient.PostAsync<object, CategoriaDto>(ENDPOINT, dto);
        }

        public async Task<ResponseDto<CategoriaDto>> ActualizarAsync(int id, object dto)
        {
            return await _httpClient.PutAsync<object, CategoriaDto>($"{ENDPOINT}/{id}", dto);
        }

        public async Task<ResponseDto<bool>> EliminarAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{ENDPOINT}/{id}");
        }
    }
}