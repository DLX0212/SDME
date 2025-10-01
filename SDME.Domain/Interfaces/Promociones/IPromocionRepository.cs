using SDME.Domain.Entities.Promociones;

namespace SDME.Domain.Interfaces.Promociones
{
    public interface IPromocionRepository : IBaseRepository<Promocion>
    {
        Task<Promocion?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Promocion>> GetVigentesAsync();
    }
}
