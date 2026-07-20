using ISARMIN.Domain.Entities.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;
using MovimientoCajaEntity = ISARMIN.Domain.Entities.Caja.MovimientoCaja;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class MovimientoCajaConfiguration : IEntityTypeConfiguration<MovimientoCajaEntity>
{
    public void Configure(EntityTypeBuilder<MovimientoCajaEntity> builder)
    {
        builder.ToTable("movimientos_caja", t =>
        {
            t.HasCheckConstraint("ck_movimientos_caja_tipo", "tipo IN ('Ingreso','Egreso')");
            t.HasCheckConstraint("ck_movimientos_caja_monto", "monto > 0");
            t.HasCheckConstraint(
                "ck_movimientos_caja_concepto",
                "concepto IN ('GastoOperativo','RetiroPropietario','AporteCapital')");
        });

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Tipo)
            .HasConversion<string>()
            .HasColumnName("tipo")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(m => m.Monto)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(m => m.Concepto)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.Descripcion)
            .HasMaxLength(255);

        builder.Property(m => m.Fecha).IsRequired();

        builder.HasOne<CajaEntity>()
            .WithMany()
            .HasForeignKey(m => m.CajaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.CajaId, m.Fecha });
    }
}
