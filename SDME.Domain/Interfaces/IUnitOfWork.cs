using SDME.Domain.Interfaces.Core;
using SDME.Domain.Interfaces.Pagos;
using SDME.Domain.Interfaces.Promociones;

namespace SDME.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work - Maneja transacciones
    /// Asegura que todas las operaciones se guarden juntas o ninguna
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repositorios del schema Core
        IUsuarioRepository Usuarios { get; }
        IProductoRepository Productos { get; }
        ICategoriaRepository Categorias { get; }
        IPedidoRepository Pedidos { get; }

        // Repositorios del schema Pagos
        IPagoRepository Pagos { get; }

        // Repositorios del schema Promociones
        IPromocionRepository Promociones { get; }

        // Guardar cambios
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
