using Microsoft.EntityFrameworkCore.Storage;
using SDME.Domain.Interfaces;
using SDME.Domain.Interfaces.Core;
using SDME.Domain.Interfaces.Pagos;
using SDME.Domain.Interfaces.Promociones;
using SDME.Persistence.Context;
using SDME.Persistence.Repositories.Core;
using SDME.Persistence.Repositories.Pagos;
using SDME.Persistence.Repositories.Promociones;

namespace SDME.Persistence
{
    /// Implementación del patrón Unit of Work este maneja transacciones y coordina repositorios

    public class UnitOfWork : IUnitOfWork
    {
        private readonly SDMEDbContext _context;
        private IDbContextTransaction? _transaction;

        // Repositorios - Lazy loading
        private IUsuarioRepository? _usuarios;
        private IProductoRepository? _productos;
        private ICategoriaRepository? _categorias;
        private IPedidoRepository? _pedidos;
        private IPagoRepository? _pagos;
        private IPromocionRepository? _promociones;

        public UnitOfWork(SDMEDbContext context)
        {
            _context = context;
        }

        #region Repositorios

        public IUsuarioRepository Usuarios
        {
            get
            {
                _usuarios ??= new UsuarioRepository(_context);
                return _usuarios;
            }
        }

        public IProductoRepository Productos
        {
            get
            {
                _productos ??= new ProductoRepository(_context);
                return _productos;
            }
        }

        public ICategoriaRepository Categorias
        {
            get
            {
                _categorias ??= new CategoriaRepository(_context);
                return _categorias;
            }
        }

        public IPedidoRepository Pedidos
        {
            get
            {
                _pedidos ??= new PedidoRepository(_context);
                return _pedidos;
            }
        }

        public IPagoRepository Pagos
        {
            get
            {
                _pagos ??= new PagoRepository(_context);
                return _pagos;
            }
        }

        public IPromocionRepository Promociones
        {
            get
            {
                _promociones ??= new PromocionRepository(_context);
                return _promociones;
            }
        }

        #endregion

        #region Transacciones

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();

                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}