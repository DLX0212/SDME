using SDME.Domain.Entities.Pagos;
using SDME.Domain.Enums;

namespace SDME.Domain.Interfaces.Pagos
{
    public interface IPagoRepository : IBaseRepository<Pago>
    {
        Task<Pago?> GetByPedidoAsync(int pedidoId);
        Task<IEnumerable<Pago>> GetByEstadoAsync(EstadoPago estado);
        Task<Pago?> GetByTransaccionIdAsync(string transaccionId);
    }
}
