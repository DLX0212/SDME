using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Promociones;
using SDME.Domain.Interfaces.Promociones;
using SDME.Persistence.Base;
using SDME.Persistence.Context;

namespace SDME.Persistence.Repositories.Promociones
{
    public class PromocionRepository : BaseRepository<Promocion>, IPromocionRepository
    {
        public PromocionRepository(SDMEDbContext context) : base(context)
        {
        }

        public async Task<Promocion?> GetByCodigoAsync(string codigo)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.CodigoCupon.ToLower() == codigo.ToLower());
        }

        public async Task<IEnumerable<Promocion>> GetVigentesAsync()
        {
            var ahora = DateTime.UtcNow;
            return await _dbSet
                .Where(p => p.FechaInicio <= ahora &&
                           p.FechaFin >= ahora &&
                           p.EstaActivo &&
                           (!p.UsosMaximos.HasValue || p.UsosActuales < p.UsosMaximos.Value))
                .ToListAsync();
        }
    }
}
