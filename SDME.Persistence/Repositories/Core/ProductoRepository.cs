using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Domain.Interfaces.Core;
using SDME.Persistence.Base;
using SDME.Persistence.Context;

namespace SDME.Persistence.Repositories.Core
{
    public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(SDMEDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Producto>> GetByCategoriaAsync(int categoriaId)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Where(p => p.CategoriaId == categoriaId && p.Disponible)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetDisponiblesAsync()
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Where(p => p.Disponible && p.Stock > 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> BuscarAsync(string termino)
        {
            return await _dbSet
                .Include(p => p.Categoria)
                .Where(p => p.Nombre.Contains(termino) ||
                           p.Descripcion.Contains(termino))
                .ToListAsync();
        }

        public async Task<bool> TieneStockAsync(int productoId, int cantidad)
        {
            var producto = await GetByIdAsync(productoId);
            return producto != null && producto.TieneStock(cantidad);
        }
    }
}