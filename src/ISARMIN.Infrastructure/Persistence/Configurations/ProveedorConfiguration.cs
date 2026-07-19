using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("proveedores", t => t.HasCheckConstraint(
            "ck_proveedores_estado",
            "estado IN ('Activo','Inactivo')"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.NombreRazonSocial)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Documento)
            .HasMaxLength(20);

        builder.Property(p => p.Telefono)
            .HasMaxLength(30);

        builder.Property(p => p.Direccion)
            .HasMaxLength(255);

        builder.Property(p => p.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoRegistro.Activo)
            .IsRequired();
    }
}
