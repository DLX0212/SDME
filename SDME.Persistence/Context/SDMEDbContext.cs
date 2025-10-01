using Microsoft.EntityFrameworkCore;
using SDME.Domain.Entities.Core;
using SDME.Domain.Entities.Notificaciones;
using SDME.Domain.Entities.Pagos;
using SDME.Domain.Entities.Promociones;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SDME.Persistence.Context
{
    /// Contexto principal de la base de datos (Configurado para PostgreSQL)

    public class SDMEDbContext : DbContext
    {
        public SDMEDbContext(DbContextOptions<SDMEDbContext> options) : base(options)
        {
        }

        #region DbSets - Schema Core
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        #endregion

        #region DbSets - Schema Pagos
        public DbSet<Pago> Pagos { get; set; }
        #endregion

        #region DbSets - Schema Promociones
        public DbSet<Promocion> Promociones { get; set; }
        #endregion

        #region DbSets - Schema Notificaciones
        public DbSet<Notificacion> Notificaciones { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas las configuraciones del assembly actual
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SDMEDbContext).Assembly);

            // Configurar schemas de PostgreSQL
            ConfigurarSchemas(modelBuilder);
        }

        private void ConfigurarSchemas(ModelBuilder modelBuilder)
        {
            // Schema Core (por defecto: public)
            modelBuilder.Entity<Usuario>().ToTable("Usuarios", "core");
            modelBuilder.Entity<Producto>().ToTable("Productos", "core");
            modelBuilder.Entity<Categoria>().ToTable("Categorias", "core");
            modelBuilder.Entity<Pedido>().ToTable("Pedidos", "core");
            modelBuilder.Entity<DetallePedido>().ToTable("DetallesPedido", "core");
            modelBuilder.Entity<Direccion>().ToTable("Direcciones", "core");
            modelBuilder.Entity<Carrito>().ToTable("Carritos", "core");

            // Schema Pagos
            modelBuilder.Entity<Pago>().ToTable("Pagos", "pagos");

            // Schema Promociones
            modelBuilder.Entity<Promocion>().ToTable("Promociones", "promociones");

            // Schema Notificaciones
            modelBuilder.Entity<Notificacion>().ToTable("Notificaciones", "notificaciones");
        }

        // Override SaveChanges para auditoría automática
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AplicarAuditoria();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AplicarAuditoria()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Domain.Base.AuditEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (Domain.Base.AuditEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.FechaCreacion = DateTime.UtcNow;
                    entity.EstaActivo = true;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.FechaModificacion = DateTime.UtcNow;
                }
            }
        }
    }
}