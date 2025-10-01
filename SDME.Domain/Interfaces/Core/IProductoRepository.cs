using SDME.Domain.Entities.Core;

namespace SDME.Domain.Interfaces.Core
{
    public interface IProductoRepository : IBaseRepository<Producto>
    {
        Task<IEnumerable<Producto>> GetByCategoriaAsync(int categoriaId);
        Task<IEnumerable<Producto>> GetDisponiblesAsync();
        Task<IEnumerable<Producto>> BuscarAsync(string termino);
        Task<bool> TieneStockAsync(int productoId, int cantidad);
    }
}
