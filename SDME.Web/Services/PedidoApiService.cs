using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Pedido;
using SDME.Web.Services.Base;
using SDME.Web.Services.Interfaces;

namespace SDME.Web.Services
{
    public class PedidoApiService : BaseApiService<PedidoDto>, IPedidoService
    {
        public PedidoApiService(IHttpClientService httpClient)
            : base(httpClient, "Pedidos")
        {
        }

        // Métodos específicos de PedidoApiService

        public async Task<ResponseDto<PedidoDto>> CrearPedidoAsync(CrearPedidoDto dto)
        {

            return await _httpClient.PostAsync<CrearPedidoDto, PedidoDto>(_endpoint, dto);
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _httpClient.GetListAsync<PedidoDto>($"{_endpoint}/usuario/{usuarioId}");
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPorEstadoAsync(string estado)
        {
            return await _httpClient.GetListAsync<PedidoDto>($"{_endpoint}/estado/{estado}");
        }

        public async Task<ResponseDto<List<PedidoDto>>> ObtenerPedidosDelDiaAsync()
        {
            return await _httpClient.GetListAsync<PedidoDto>($"{_endpoint}/hoy");
        }

        public async Task<ResponseDto<PedidoDto>> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto)
        {

            return await _httpClient.PatchAsync<ActualizarEstadoPedidoDto, PedidoDto>(
                $"{_endpoint}/{id}/estado",
                dto);
        }
    }
}