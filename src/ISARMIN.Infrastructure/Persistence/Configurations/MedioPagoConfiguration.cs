using ISARMIN.Domain.Entities.Configuracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class MedioPagoConfiguration : IEntityTypeConfiguration<MedioPago>
{
    public void Configure(EntityTypeBuilder<MedioPago> builder)
    {
        builder.ToTable("medios_pago");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombre)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(m => m.Nombre).IsUnique();

        builder.Property(m => m.Activo)
            .HasDefaultValue(true)
            .IsRequired();

        // Semilla inicial confirmada — RF-042, CAT-008 (resuelta).
        builder.HasData(
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000c1"), Nombre = "Efectivo", Activo = true },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000c2"), Nombre = "Yape", Activo = true },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000c3"), Nombre = "Plin", Activo = true },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000c4"), Nombre = "Transferencia bancaria", Activo = true });
    }
}
