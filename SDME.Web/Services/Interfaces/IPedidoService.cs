using SDME.Application.DTOs.Common;
using SDME.Application.DTOs.Pedido;

namespace SDME.Web.Services.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de Pedidos
    /// Hereda operaciones CRUD + métodos específicos de pedidos
    /// </summary>
    public interface IPedidoService : IApiService<PedidoDto>
    {
        // Métodos específicos de pedidos
        Task<ResponseDto<PedidoDto>> CrearPedidoAsync(CrearPedidoDto dto);
        Task<ResponseDto<List<PedidoDto>>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<ResponseDto<List<PedidoDto>>> ObtenerPorEstadoAsync(string estado);
        Task<ResponseDto<List<PedidoDto>>> ObtenerPedidosDelDiaAsync();
        Task<ResponseDto<PedidoDto>> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto);
    }
}