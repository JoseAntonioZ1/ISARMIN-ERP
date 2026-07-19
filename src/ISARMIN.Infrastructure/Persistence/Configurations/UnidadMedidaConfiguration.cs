using ISARMIN.Domain.Entities.Inventario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class UnidadMedidaConfiguration : IEntityTypeConfiguration<UnidadMedida>
{
    public void Configure(EntityTypeBuilder<UnidadMedida> builder)
    {
        builder.ToTable("unidades_medida");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(u => u.Nombre).IsUnique();

        // Semilla inicial confirmada (RN-040, CAT-014 resuelta) — catálogo ampliable por el Administrador.
        builder.HasData(
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000d1"), Nombre = "Unidad" },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000d2"), Nombre = "Metro" },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000d3"), Nombre = "Kilogramo" },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000d4"), Nombre = "Litro" },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000d5"), Nombre = "Rollo" },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000d6"), Nombre = "Par" },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000d7"), Nombre = "Juego" });
    }
}
