using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Pedido;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{
    /// <summary>
    /// Servicio de pedidos - USA la abstracción IHttpClientService
    /// </summary>
    public class PedidoApiService : IPedidoService
    {
        private readonly IHttpClientService _httpClient;
        private readonly ILogger<PedidoApiService> _logger;
        private const string ENDPOINT = "Pedidos";

        public PedidoApiService(
            IHttpClientService httpClient,
            ILogger<PedidoApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // ✅ Métodos CRUD heredados de IApiService<PedidoDto>
        public async Task<ResponseDto<List<PedidoDto>>> ObtenerTodosAsync()
        {
            return await _httpClient.GetListAsync<PedidoDto>(ENDPOINT);
        }

        public async Task<ResponseDto<PedidoDto>> ObtenerPorIdAsync(int id)
        {
            return await _httpClient.GetAsync<PedidoDto>($"{ENDPOINT}/{id}");
        }

        public async Task<ResponseDto<PedidoDto>> CrearAsync(object dto)
        {
            return await _httpClient.PostAsync<object, PedidoDto>(ENDPOINT, dto);
        }

        public async Task<ResponseDto<PedidoDto>> ActualizarAsync(int id, object dto)
        {
            return await _httpClient.PutAsync<object, PedidoDto>($"{ENDPOINT}/{id}", dto);
        }

        public async Task<ResponseDto<bool>> EliminarAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{ENDPOINT}/{id}");
        }

        // ✅ Métodos específicos de IPedidoService
        public async Task<ResponseDto<PedidoDto>> CrearPedidoAsync(CrearPedidoDto dto)
        {
            return await _httpClient.PostAsync<CrearPedidoDto, PedidoDto>(ENDPOINT, dto);
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _httpClient.GetListAsync<PedidoDto>($"{ENDPOINT}/usuario/{usuarioId}");
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorEstadoAsync(string estado)
        {
            return await _httpClient.GetListAsync<PedidoDto>($"{ENDPOINT}/estado/{estado}");
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPedidosDelDiaAsync()
        {
            return await _httpClient.GetListAsync<PedidoDto>($"{ENDPOINT}/hoy");
        }

        public async Task<ResponseDto<PedidoDto>> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto)
        {
            return await _httpClient.PatchAsync<ActualizarEstadoPedidoDto, PedidoDto>(
                $"{ENDPOINT}/{id}/estado",
                dto);
        }
    }
}