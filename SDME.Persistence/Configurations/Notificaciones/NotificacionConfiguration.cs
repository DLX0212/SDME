using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDME.Domain.Entities.Notificaciones;

namespace SDME.Persistence.Configurations.Notificaciones
{
    public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
    {
        public void Configure(EntityTypeBuilder<Notificacion> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Tipo)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(n => n.Asunto)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Mensaje)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(n => n.Enviada)
                .HasDefaultValue(false);

            builder.Property(n => n.Leida)
                .HasDefaultValue(false);

            // Relaciones
            builder.HasOne(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Pedido)
                .WithMany()
                .HasForeignKey(n => n.PedidoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices
            builder.HasIndex(n => n.Enviada);
            builder.HasIndex(n => n.Leida);
            builder.HasIndex(n => n.FechaEnvio);
        }
    }
}
