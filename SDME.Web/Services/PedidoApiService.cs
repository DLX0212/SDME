using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Pedido;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SDME.Web.Services
{

    /// Servicio para consumir endpoints de Pedidos

    public class PedidoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PedidoApiService> _logger;
        private const string ENDPOINT = "Pedidos";

        public PedidoApiService(
            IHttpClientFactory httpClientFactory,
            ILogger<PedidoApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("SDMEAPI");
            _logger = logger;
        }

 
        /// Crea un nuevo pedido

        public async Task<ResponseDto<PedidoDto>> CrearPedidoAsync(CrearPedidoDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ENDPOINT, content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<PedidoDto>>();
                    return resultado ?? ResponseDto<PedidoDto>.Failure("Error al crear pedido");
                }

                // Intentar leer el mensaje de error de la API
                try
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ResponseDto<PedidoDto>>();
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Mensaje))
                    {
                        return errorResponse;
                    }
                }
                catch { }

                return ResponseDto<PedidoDto>.Failure("Error al crear el pedido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear pedido");
                return ResponseDto<PedidoDto>.Failure("Error de conexión con la API");
            }
        }


        /// Obtiene un pedido por ID

        public async Task<ResponseDto<PedidoDto>> ObtenerPorIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<PedidoDto>>();
                    return resultado ?? ResponseDto<PedidoDto>.Failure("Pedido no encontrado");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<PedidoDto>.Failure("Pedido no encontrado");
                }

                return ResponseDto<PedidoDto>.Failure("Error al obtener pedido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedido {PedidoId}", id);
                return ResponseDto<PedidoDto>.Failure("Error de conexión con la API");
            }
        }

        /// Obtiene pedidos por usuario

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/usuario/{usuarioId}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<PedidoDto>>>();
                    return resultado ?? ResponseDto<List<PedidoDto>>.Failure("No se encontraron pedidos");
                }

                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener pedidos del usuario");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del usuario {UsuarioId}", usuarioId);
                return ResponseDto<List<PedidoDto>>.Failure("Error de conexión con la API");
            }
        }

        /// Obtiene pedidos por estado

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorEstadoAsync(string estado)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/estado/{estado}");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<PedidoDto>>>();
                    return resultado ?? ResponseDto<List<PedidoDto>>.Failure("No se encontraron pedidos");
                }

                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener pedidos por estado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos por estado {Estado}", estado);
                return ResponseDto<List<PedidoDto>>.Failure("Error de conexión con la API");
            }
        }

        /// Obtiene pedidos del día actual

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPedidosDelDiaAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ENDPOINT}/hoy");

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<List<PedidoDto>>>();
                    return resultado ?? ResponseDto<List<PedidoDto>>.Failure("No se encontraron pedidos");
                }

                return ResponseDto<List<PedidoDto>>.Failure("Error al obtener pedidos del día");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pedidos del día");
                return ResponseDto<List<PedidoDto>>.Failure("Error de conexión con la API");
            }
        }


        /// Actualiza el estado de un pedido
       
        public async Task<ResponseDto<PedidoDto>> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto)
        {
            try
            {
                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"{ENDPOINT}/{id}/estado", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<ResponseDto<PedidoDto>>();
                    return resultado ?? ResponseDto<PedidoDto>.Failure("Error al actualizar estado");
                }

                // Intentar leer mensaje de error
                try
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<ResponseDto<PedidoDto>>();
                    if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Mensaje))
                    {
                        return errorResponse;
                    }
                }
                catch { }

                return ResponseDto<PedidoDto>.Failure("Error al actualizar el estado del pedido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado del pedido {PedidoId}", id);
                return ResponseDto<PedidoDto>.Failure("Error de conexión con la API");
            }
        }
    }
}