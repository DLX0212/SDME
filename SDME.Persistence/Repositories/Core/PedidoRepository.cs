using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Domain.Enums;
using SDME.Domain.Interfaces.Core;
using SDME.Persistence.Base;
using SDME.Persistence.Context;

namespace SDME.Persistence.Repositories.Core
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        public PedidoRepository(SDMEDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Pedido>> GetByUsuarioAsync(int usuarioId)
        {
            return await _dbSet
                .Include(p => p.DetallesPedido)
                    .ThenInclude(d => d.Producto)
                .Include(p => p.DireccionEntrega)
                .Include(p => p.Pago)
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> GetByEstadoAsync(EstadoPedido estado)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.DetallesPedido)
                    .ThenInclude(d => d.Producto)
                .Where(p => p.Estado == estado)
                .OrderBy(p => p.FechaPedido)
                .ToListAsync();
        }

        public async Task<Pedido?> GetConDetallesAsync(int pedidoId)
        {
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.DetallesPedido)
                    .ThenInclude(d => d.Producto)
                        .ThenInclude(pr => pr.Categoria)
                .Include(p => p.DireccionEntrega)
                .Include(p => p.Pago)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);
        }

        public async Task<IEnumerable<Pedido>> GetPedidosDelDiaAsync()
        {
            var hoy = DateTime.UtcNow.Date;
            return await _dbSet
                .Include(p => p.Usuario)
                .Include(p => p.DetallesPedido)
                .Where(p => p.FechaPedido.Date == hoy)
                .ToListAsync();
        }

        public async Task<decimal> GetVentasTotalesAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _dbSet
                .Where(p => p.FechaPedido >= fechaInicio &&
                           p.FechaPedido <= fechaFin &&
                           p.Estado == EstadoPedido.Entregado)
                .SumAsync(p => p.Total);
        }
    }
}
