using Microsoft.Extensions.Logging;
using SDME.Application.DTOs.Common;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace SDME.Web.Services.Base
{

    /// Implementación del cliente HTTP
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public HttpClientService(
            IHttpClientFactory httpClientFactory,
            ILogger<HttpClientService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("SDMEAPI");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ResponseDto<T>> GetAsync<T>(string endpoint) where T : class
        {
            try
            {
                _logger.LogInformation("GET request: {Endpoint}", endpoint);

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseDto<T>>(_jsonOptions);
                    return result ?? ResponseDto<T>.Failure("No se recibieron datos");
                }

                return HandleErrorResponse<T>(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en GET {Endpoint}", endpoint);
                return ResponseDto<T>.Failure("Error de conexión con la API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en GET {Endpoint}", endpoint);
                return ResponseDto<T>.Failure("Error inesperado al procesar la solicitud");
            }
        }

        public async Task<ResponseDto<List<T>>> GetListAsync<T>(string endpoint) where T : class
        {
            try
            {
                _logger.LogInformation("GET List request: {Endpoint}", endpoint);

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseDto<List<T>>>(_jsonOptions);
                    return result ?? ResponseDto<List<T>>.Failure("No se recibieron datos");
                }

                return HandleErrorResponse<List<T>>(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en GET List {Endpoint}", endpoint);
                return ResponseDto<List<T>>.Failure("Error de conexión con la API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en GET List {Endpoint}", endpoint);
                return ResponseDto<List<T>>.Failure("Error inesperado al procesar la solicitud");
            }
        }

        public async Task<ResponseDto<TResponse>> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest payload) where TResponse : class
        {
            try
            {
                _logger.LogInformation("POST request: {Endpoint}", endpoint);

                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseDto<TResponse>>(_jsonOptions);
                    return result ?? ResponseDto<TResponse>.Failure("Error al procesar respuesta");
                }

                return await TryReadErrorResponseAsync<TResponse>(response)
                    ?? HandleErrorResponse<TResponse>(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en POST {Endpoint}", endpoint);
                return ResponseDto<TResponse>.Failure("Error de conexión con la API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en POST {Endpoint}", endpoint);
                return ResponseDto<TResponse>.Failure("Error inesperado al procesar la solicitud");

            }
        }

        public async Task<ResponseDto<TPrimitive>> GetPrimitiveAsync<TPrimitive>(string endpoint)
        {
            try
            {
                _logger.LogInformation("GET Primitive request: {Endpoint}", endpoint);

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseDto<TPrimitive>>(_jsonOptions);
                    return result ?? ResponseDto<TPrimitive>.Failure("No se recibieron datos");
                }

                // Manejo especial para bool
                if (typeof(TPrimitive) == typeof(bool))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return (ResponseDto<TPrimitive>)(object)ResponseDto<bool>.Success(false);
                    }
                }

                return HandleErrorResponsePrimitive<TPrimitive>(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en GET Primitive {Endpoint}", endpoint);
                return ResponseDto<TPrimitive>.Failure("Error de conexión con la API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en GET Primitive {Endpoint}", endpoint);
                return ResponseDto<TPrimitive>.Failure("Error inesperado al procesar la solicitud");
            }
        }


        private ResponseDto<TPrimitive> HandleErrorResponsePrimitive<TPrimitive>(HttpResponseMessage response)
        {
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ResponseDto<TPrimitive>.Failure("Recurso no encontrado"),
                System.Net.HttpStatusCode.Unauthorized => ResponseDto<TPrimitive>.Failure("No autorizado"),
                System.Net.HttpStatusCode.BadRequest => ResponseDto<TPrimitive>.Failure("Solicitud inválida"),
                _ => ResponseDto<TPrimitive>.Failure($"Error del servidor: {response.StatusCode}")
            };
        }

        public async Task<ResponseDto<TResponse>> PutAsync<TRequest, TResponse>(
            string endpoint,
            TRequest payload) where TResponse : class
        {
            try
            {
                _logger.LogInformation("PUT request: {Endpoint}", endpoint);

                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseDto<TResponse>>(_jsonOptions);
                    return result ?? ResponseDto<TResponse>.Failure("Error al procesar respuesta");
                }

                return HandleErrorResponse<TResponse>(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en PUT {Endpoint}", endpoint);
                return ResponseDto<TResponse>.Failure("Error de conexión con la API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en PUT {Endpoint}", endpoint);
                return ResponseDto<TResponse>.Failure("Error inesperado al procesar la solicitud");
            }
        }

        public async Task<ResponseDto<TResponse>> PatchAsync<TRequest, TResponse>(
            string endpoint,
            TRequest payload) where TResponse : class
        {
            try
            {
                _logger.LogInformation("PATCH request: {Endpoint}", endpoint);

                var json = JsonSerializer.Serialize(payload, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseDto<TResponse>>(_jsonOptions);
                    return result ?? ResponseDto<TResponse>.Failure("Error al procesar respuesta");
                }

                return await TryReadErrorResponseAsync<TResponse>(response)
                    ?? HandleErrorResponse<TResponse>(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en PATCH {Endpoint}", endpoint);
                return ResponseDto<TResponse>.Failure("Error de conexión con la API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en PATCH {Endpoint}", endpoint);
                return ResponseDto<TResponse>.Failure("Error inesperado al procesar la solicitud");
            }
        }

        public async Task<ResponseDto<bool>> DeleteAsync(string endpoint)
        {
            try
            {
                _logger.LogInformation("DELETE request: {Endpoint}", endpoint);

                var response = await _httpClient.DeleteAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseDto<bool>>(_jsonOptions);
                    return result ?? ResponseDto<bool>.Success(true);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ResponseDto<bool>.Failure("Recurso no encontrado");
                }

                return ResponseDto<bool>.Failure($"Error al eliminar: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en DELETE {Endpoint}", endpoint);
                return ResponseDto<bool>.Failure("Error de conexión con la API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en DELETE {Endpoint}", endpoint);
                return ResponseDto<bool>.Failure("Error inesperado al procesar la solicitud");
            }
        }

        /// Maneja respuestas de error estándar
        private ResponseDto<T> HandleErrorResponse<T>(HttpResponseMessage response) where T : class
        {
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ResponseDto<T>.Failure("Recurso no encontrado"),
                System.Net.HttpStatusCode.Unauthorized => ResponseDto<T>.Failure("No autorizado"),
                System.Net.HttpStatusCode.BadRequest => ResponseDto<T>.Failure("Solicitud inválida"),
                _ => ResponseDto<T>.Failure($"Error del servidor: {response.StatusCode}")
            };
        }


        /// Intenta leer un mensaje de error específico de la respuesta
        private async Task<ResponseDto<T>?> TryReadErrorResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            try
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ResponseDto<T>>(_jsonOptions);
                if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Mensaje))
                {
                    return errorResponse;
                }
            }
            catch
            {
                // Si falla, retornar null
            }

            return null;
        }
    }
}

