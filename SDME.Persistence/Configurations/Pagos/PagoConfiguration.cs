using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDME.Domain.Entities.Pagos;

namespace SDME.Persistence.Configurations.Pagos
{
    public class PagoConfiguration : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.MetodoPago)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.Monto)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(p => p.Estado)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.TransaccionId)
                .HasMaxLength(200);

            builder.Property(p => p.ReferenciaPasarela)
                .HasMaxLength(500);

            builder.Property(p => p.MensajeError)
                .HasMaxLength(1000);

            // Relación
            builder.HasOne(p => p.Pedido)
                .WithOne(pd => pd.Pago)
                .HasForeignKey<Pago>(p => p.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(p => p.TransaccionId);
            builder.HasIndex(p => p.Estado);
        }
    }
}