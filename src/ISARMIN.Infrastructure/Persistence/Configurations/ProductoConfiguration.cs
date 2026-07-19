using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("productos", t => t.HasCheckConstraint(
            "ck_productos_estado",
            "estado IN ('Activo','Inactivo')"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CodigoInterno)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(p => p.CodigoInterno).IsUnique();

        builder.Property(p => p.CodigoBarras)
            .HasMaxLength(50);
        builder.HasIndex(p => p.CodigoBarras).IsUnique().HasFilter("codigo_barras IS NOT NULL");

        builder.Property(p => p.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Marca)
            .HasMaxLength(100);

        builder.Property(p => p.CostoReferencia)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.PrecioVenta)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.StockActual)
            .HasPrecision(12, 3)
            .IsRequired();

        builder.Property(p => p.StockMinimo)
            .HasPrecision(12, 3);

        builder.Property(p => p.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoRegistro.Activo)
            .IsRequired();

        builder.Ignore(p => p.Margen);

        builder.HasOne<Categoria>()
            .WithMany()
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UnidadMedida>()
            .WithMany()
            .HasForeignKey(p => p.UnidadMedidaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
