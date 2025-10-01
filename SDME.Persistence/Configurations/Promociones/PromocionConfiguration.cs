using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDME.Domain.Entities.Promociones;

namespace SDME.Persistence.Configurations.Promociones
{
    public class PromocionConfiguration : IEntityTypeConfiguration<Promocion>
    {
        public void Configure(EntityTypeBuilder<Promocion> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Descripcion)
                .HasMaxLength(1000);

            builder.Property(p => p.CodigoCupon)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.CodigoCupon)
                .IsUnique();

            builder.Property(p => p.PorcentajeDescuento)
                .HasPrecision(5, 2);

            builder.Property(p => p.MontoDescuento)
                .HasPrecision(10, 2);

            builder.Property(p => p.FechaInicio)
                .IsRequired();

            builder.Property(p => p.FechaFin)
                .IsRequired();

            builder.Property(p => p.UsosActuales)
                .HasDefaultValue(0);

            builder.Property(p => p.MontoMinimoCompra)
                .HasPrecision(10, 2);

            // Índices
            builder.HasIndex(p => p.FechaInicio);
            builder.HasIndex(p => p.FechaFin);
        }
    }
}