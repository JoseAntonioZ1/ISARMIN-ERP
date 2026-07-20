using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.ServiciosCampo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class ServicioCampoDetalleConfiguration : IEntityTypeConfiguration<ServicioCampoDetalle>
{
    public void Configure(EntityTypeBuilder<ServicioCampoDetalle> builder)
    {
        builder.ToTable("servicios_campo_detalle", t => t.HasCheckConstraint(
            "ck_servicios_campo_detalle_cantidad",
            "cantidad > 0"));

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Cantidad).HasPrecision(12, 3).IsRequired();

        builder.HasOne<ServicioCampo>()
            .WithMany(s => s.Detalles)
            .HasForeignKey(d => d.ServicioCampoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
