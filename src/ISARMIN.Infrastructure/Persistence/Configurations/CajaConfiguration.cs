using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class CajaConfiguration : IEntityTypeConfiguration<CajaEntity>
{
    public void Configure(EntityTypeBuilder<CajaEntity> builder)
    {
        builder.ToTable("cajas", t => t.HasCheckConstraint(
            "ck_cajas_estado",
            "estado IN ('Abierta','Cerrada')"));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FechaApertura).IsRequired();

        builder.Property(c => c.MontoApertura)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(c => c.MontoTeoricoCierre).HasPrecision(12, 2);
        builder.Property(c => c.MontoFisicoDeclarado).HasPrecision(12, 2);

        builder.Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoCaja.Abierta)
            .IsRequired();

        builder.Ignore(c => c.EstaAbierta);
        builder.Ignore(c => c.Diferencia);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
