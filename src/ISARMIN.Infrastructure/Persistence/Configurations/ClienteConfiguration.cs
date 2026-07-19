using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes", t =>
        {
            t.HasCheckConstraint("ck_clientes_tipo_cliente", "tipo_cliente IS NULL OR tipo_cliente IN ('Natural','Juridica')");
            t.HasCheckConstraint("ck_clientes_estado", "estado IN ('Activo','Inactivo')");
        });

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NombreRazonSocial)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Telefono)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.Direccion)
            .HasMaxLength(255);

        builder.Property(c => c.TipoDocumento)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.NumeroDocumento)
            .HasMaxLength(20);

        builder.Property(c => c.TipoCliente)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoRegistro.Activo)
            .IsRequired();
    }
}
