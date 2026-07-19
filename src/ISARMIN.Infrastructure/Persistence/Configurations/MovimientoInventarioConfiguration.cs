using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Inventario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
{
    public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
    {
        builder.ToTable("movimientos_inventario", t =>
        {
            t.HasCheckConstraint(
                "ck_movimientos_inventario_tipo_movimiento",
                "tipo_movimiento IN ('Compra','Venta','ConsumoTaller','ConsumoCampo','Ajuste','Devolucion')");
            t.HasCheckConstraint(
                "ck_movimientos_inventario_origen_tipo",
                "origen_tipo IN ('Compra','Venta','OrdenTrabajo','ServicioCampo') OR origen_tipo IS NULL");
        });

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TipoMovimiento)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Cantidad)
            .HasPrecision(12, 3)
            .IsRequired();

        builder.Property(m => m.OrigenTipo)
            .HasMaxLength(20);

        builder.Property(m => m.MotivoAjuste)
            .HasColumnType("text");

        builder.Property(m => m.Fecha)
            .IsRequired();

        builder.HasOne<Producto>()
            .WithMany()
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.ProductoId, m.Fecha });
    }
}
