using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDME.Domain.Entities.Core;

namespace SDME.Persistence.Configurations.Core
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.NumeroPedido)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.NumeroPedido)
                .IsUnique();

            builder.Property(p => p.FechaPedido)
                .IsRequired();

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Subtotal)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.Impuesto)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.Total)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.NotasEspeciales)
                .HasMaxLength(500);

            builder.Property(p => p.TipoEntrega)
                .IsRequired()
                .HasConversion<int>();

            // Relaciones
            builder.HasOne(p => p.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.DireccionEntrega)
                .WithMany(d => d.Pedidos)
                .HasForeignKey(p => p.DireccionEntregaId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.DetallesPedido)
                .WithOne(d => d.Pedido)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Pago)
                .WithOne(pg => pg.Pedido)
                .HasForeignKey<Domain.Entities.Pagos.Pago>(pg => pg.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(p => p.Estado);
            builder.HasIndex(p => p.FechaPedido);
        }
    }
}
