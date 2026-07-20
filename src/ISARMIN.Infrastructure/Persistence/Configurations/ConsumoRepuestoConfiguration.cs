using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Taller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class ConsumoRepuestoConfiguration : IEntityTypeConfiguration<ConsumoRepuesto>
{
    public void Configure(EntityTypeBuilder<ConsumoRepuesto> builder)
    {
        builder.ToTable("consumos_repuesto", t => t.HasCheckConstraint(
            "ck_consumos_repuesto_cantidad",
            "cantidad > 0"));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Cantidad).HasPrecision(12, 3).IsRequired();

        builder.HasOne<OrdenTrabajo>()
            .WithMany(o => o.ConsumosRepuesto)
            .HasForeignKey(c => c.OrdenTrabajoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(c => c.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
