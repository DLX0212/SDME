using SDME.Application.DTOs.Categoria;
using SDME.Application.DTOs.Common;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SDME.Web.Services
{

    /// Servicio para consumir endpoints de Categorías

    public class CategoriaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoriaApiService> _logger;
        private const string ENDPOINT = "Categorias";

        public CategoriaApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<CategoriaApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("SDMEAPI");
            _logger = logger;
        }


        /// Obtiene todas las categorías con sus productos

        public async Task<ResponseDto<List<CategoriaDto>>> ObtenerTodasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ENDPOINT);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<CategoriaDto>>>();
                    return resultado ?? ResponseDto<List<CategoriaDto>>.Failure("No se recibieron datos");
                }

                _logger.LogWarning("Error al obtener categorías. Status: {StatusCode}", response.StatusCode);
                return ResponseDto<List<CategoriaDto>>.Failure("Error al obtener categorías");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return ResponseDto<List<CategoriaDto>>.Failure("Error de conexión con la API");
            }
        }


        /// Obtiene una categoría por ID

        public async Task<ResponseDto<CategoriaDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<CategoriaDto>>();
                    return resultado ?? ResponseDto<CategoriaDto>.Failure("Categoría no encontrada");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<CategoriaDto>.Failure("Categoría no encontrada");
                }

                return ResponseDto<CategoriaDto>.Failure("Error al obtener categoría");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categoría {CategoriaId}", id);
                return ResponseDto<CategoriaDto>.Failure("Error de conexión con la API");
            }
        }


        /// Crea una nueva categoría

        public async Task<ResponseDto<CategoriaDto>> CrearAsync(CrearCategoriaDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ENDPOINT, content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<CategoriaDto>>();
                    return resultado ?? ResponseDto<CategoriaDto>.Failure("Error al crear categoría");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al crear categoría: {Error}", errorContent);

                return ResponseDto<CategoriaDto>.Failure("Error al crear la categoría");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return ResponseDto<CategoriaDto>.Failure("Error de conexión con la API");
            }
        }
    }
}