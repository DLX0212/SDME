using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;

namespace SDME.Domain.Interfaces.Core
{
    public interface IPedidoRepository : IBaseRepository<Pedido>
    {
        Task<IEnumerable<Pedido>> GetByUsuarioAsync(int usuarioId);
        Task<IEnumerable<Pedido>> GetByEstadoAsync(EstadoPedido estado);
        Task<Pedido?> GetConDetallesAsync(int pedidoId);
        Task<IEnumerable<Pedido>> GetPedidosDelDiaAsync();
        Task<decimal> GetVentasTotalesAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}
