using ISARMIN.Domain.Entities.Compras;
using ISARMIN.Domain.Entities.Inventario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class CompraDetalleConfiguration : IEntityTypeConfiguration<CompraDetalle>
{
    public void Configure(EntityTypeBuilder<CompraDetalle> builder)
    {
        builder.ToTable("compra_detalle", t => t.HasCheckConstraint(
            "ck_compra_detalle_cantidad",
            "cantidad > 0"));

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Cantidad)
            .HasPrecision(12, 3)
            .IsRequired();

        builder.Property(d => d.CostoUnitario)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.HasOne<Compra>()
            .WithMany(c => c.Detalles)
            .HasForeignKey(d => d.CompraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
