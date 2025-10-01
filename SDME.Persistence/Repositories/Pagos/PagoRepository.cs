using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Pagos;
using SDME.Domain.Enums;
using SDME.Domain.Interfaces.Pagos;
using SDME.Persistence.Base;
using SDME.Persistence.Context;

namespace SDME.Persistence.Repositories.Pagos
{
    public class PagoRepository : BaseRepository<Pago>, IPagoRepository
    {
        public PagoRepository(SDMEDbContext context) : base(context)
        {
        }

        public async Task<Pago?> GetByPedidoAsync(int pedidoId)
        {
            return await _dbSet
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
        }

        public async Task<IEnumerable<Pago>> GetByEstadoAsync(EstadoPago estado)
        {
            return await _dbSet
                .Include(p => p.Pedido)
                    .ThenInclude(pd => pd.Usuario)
                .Where(p => p.Estado == estado)
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Pago?> GetByTransaccionIdAsync(string transaccionId)
        {
            return await _dbSet
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.TransaccionId == transaccionId);
        }
    }
}
