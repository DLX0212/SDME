using SDME.Domain.Entities.Core;

namespace SDME.Domain.Interfaces.Core
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetConProductosAsync();
        Task<Categoria?> GetByNombreAsync(string nombre);
    }
}