using SDME.Application.DTOs.Pedido;
using SDME.Application.DTOs.Common;

namespace SDME.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<ResponseDto<PedidoDto>> CrearPedidoAsync(CrearPedidoDto dto);
        Task<ResponseDto<PedidoDto>> ObtenerPorIdAsync(int id);
        Task<ResponseDto<List<PedidoDto>>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<ResponseDto<List<PedidoDto>>> ObtenerPorEstadoAsync(string estado);
        Task<ResponseDto<PedidoDto>> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto);
        Task<ResponseDto<List<PedidoDto>>> ObtenerPedidosDelDiaAsync();
    }
}
