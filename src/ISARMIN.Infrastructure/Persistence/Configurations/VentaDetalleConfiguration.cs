using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class VentaDetalleConfiguration : IEntityTypeConfiguration<VentaDetalle>
{
    public void Configure(EntityTypeBuilder<VentaDetalle> builder)
    {
        builder.ToTable("venta_detalle", t => t.HasCheckConstraint(
            "ck_venta_detalle_cantidad",
            "cantidad > 0"));

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Cantidad).HasPrecision(12, 3).IsRequired();
        builder.Property(d => d.PrecioUnitario).HasPrecision(12, 2).IsRequired();

        builder.HasOne<Venta>()
            .WithMany(v => v.Detalles)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
