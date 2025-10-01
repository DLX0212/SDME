using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDME.Domain.Entities.Core;

namespace SDME.Persistence.Configurations.Core
{
    public class CarritoConfiguration : IEntityTypeConfiguration<Carrito>
    {
        public void Configure(EntityTypeBuilder<Carrito> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Cantidad)
                .IsRequired();

            // Relaciones
            builder.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Producto)
                .WithMany()
                .HasForeignKey(c => c.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice compuesto (un usuario no puede tener el mismo producto 2 veces en el carrito)
            builder.HasIndex(c => new { c.UsuarioId, c.ProductoId })
                .IsUnique();
        }
    }
}