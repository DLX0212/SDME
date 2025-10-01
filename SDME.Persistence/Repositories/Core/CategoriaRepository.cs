using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Domain.Interfaces.Core;
using SDME.Persistence.Base;
using SDME.Persistence.Context;

namespace SDME.Persistence.Repositories.Core
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(SDMEDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Categoria>> GetConProductosAsync()
        {
            return await _dbSet
                .Include(c => c.Productos.Where(p => p.Disponible))
                .Where(c => c.EstaActivo)
                .OrderBy(c => c.Orden)
                .ToListAsync();
        }

        public async Task<Categoria?> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower());
        }
    }
}
