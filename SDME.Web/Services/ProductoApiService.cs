using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Producto;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SDME.Web.Services
{

    /// Servicio para consumir los endpoints de Productos de la API

    public class ProductoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductoApiService> _logger;
        private const string ENDPOINT = "Productos";

        public ProductoApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<ProductoApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("SDMEAPI");
            _logger = logger;
        }


        /// Obtiene todos los productos

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerTodosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ENDPOINT);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<ProductoDto>>>();
                    return resultado ?? ResponseDto<List<ProductoDto>>.Failure("No se recibieron datos");
                }

                _logger.LogWarning("Error al obtener productos. Status: {StatusCode}", response.StatusCode);
                return ResponseDto<List<ProductoDto>>.Failure($"Error al obtener productos: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al obtener productos");
                return ResponseDto<List<ProductoDto>>.Failure("Error de conexión con la API");
            }
        }

        /// Obtiene un producto por ID

        public async Task<ResponseDto<ProductoDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<ProductoDto>>();
                    return resultado ?? ResponseDto<ProductoDto>.Failure("Producto no encontrado");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<ProductoDto>.Failure("Producto no encontrado");
                }

                return ResponseDto<ProductoDto>.Failure($"Error al obtener producto: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto {ProductoId}", id);
                return ResponseDto<ProductoDto>.Failure("Error de conexión con la API");
            }
        }

        /// Obtiene productos por categoría

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/categoria/{categoriaId}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<ProductoDto>>>();
                    return resultado ?? ResponseDto<List<ProductoDto>>.Failure("No se encontraron productos");
                }

                return ResponseDto<List<ProductoDto>>.Failure("Error al obtener productos por categoría");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos por categoría {CategoriaId}", categoriaId);
                return ResponseDto<List<ProductoDto>>.Failure("Error de conexión con la API");
            }
        }


        /// Obtiene productos disponibles (con stock)

        public async Task<ResponseDto<List<ProductoDto>>> ObtenerDisponiblesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/disponibles");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<ProductoDto>>>();
                    return resultado ?? ResponseDto<List<ProductoDto>>.Failure("No se encontraron productos");
                }

                return ResponseDto<List<ProductoDto>>.Failure("Error al obtener productos disponibles");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos disponibles");
                return ResponseDto<List<ProductoDto>>.Failure("Error de conexión con la API");
            }
        }


        /// Busca productos por término

        public async Task<ResponseDto<List<ProductoDto>>> BuscarAsync(string termino)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/buscar?termino={Uri.EscapeDataString(termino)}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<ProductoDto>>>();
                    return resultado ?? ResponseDto<List<ProductoDto>>.Failure("No se encontraron productos");
                }

                return ResponseDto<List<ProductoDto>>.Failure("Error al buscar productos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos con término: {Termino}", termino);
                return ResponseDto<List<ProductoDto>>.Failure("Error de conexión con la API");
            }
        }


        /// Crea un nuevo producto (Solo admin)

        public async Task<ResponseDto<ProductoDto>> CrearAsync(CrearProductoDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ENDPOINT, content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<ProductoDto>>();
                    return resultado ?? ResponseDto<ProductoDto>.Failure("Error al crear producto");
                }

                // Leer mensaje de error de la API
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al crear producto: {Error}", errorContent);

                return ResponseDto<ProductoDto>.Failure("Error al crear el producto");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al crear producto");
                return ResponseDto<ProductoDto>.Failure("Error de conexión con la API");
            }
        }


        /// Actualiza un producto existente

        public async Task<ResponseDto<ProductoDto>> ActualizarAsync(int id, ActualizarProductoDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{ENDPOINT}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<ProductoDto>>();
                    return resultado ?? ResponseDto<ProductoDto>.Failure("Error al actualizar producto");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<ProductoDto>.Failure("Producto no encontrado");
                }

                return ResponseDto<ProductoDto>.Failure("Error al actualizar el producto");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {ProductoId}", id);
                return ResponseDto<ProductoDto>.Failure("Error de conexión con la API");
            }
        }

        /// Elimina un producto (eliminación lógica)

        public async Task<ResponseDto<bool>> EliminarAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ENDPOINT}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<bool>>();
                    return resultado ?? ResponseDto<bool>.Success(true, "Producto eliminado");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<bool>.Failure("Producto no encontrado");
                }

                return ResponseDto<bool>.Failure("Error al eliminar el producto");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {ProductoId}", id);
                return ResponseDto<bool>.Failure("Error de conexión con la API");
            }
        }
    }
}
