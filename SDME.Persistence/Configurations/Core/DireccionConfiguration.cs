using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDME.Domain.Entities.Core;

namespace SDME.Persistence.Configurations.Core
{
    public class DireccionConfiguration : IEntityTypeConfiguration<Direccion>
    {
        public void Configure(EntityTypeBuilder<Direccion> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.NombreDireccion)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Calle)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Numero)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.Sector)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Ciudad)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Referencia)
                .HasMaxLength(500);

            builder.Property(d => d.EsPrincipal)
                .HasDefaultValue(false);

            // Relación
            builder.HasOne(d => d.Usuario)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
