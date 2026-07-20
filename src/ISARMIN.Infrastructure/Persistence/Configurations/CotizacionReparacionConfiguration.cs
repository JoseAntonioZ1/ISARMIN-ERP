using ISARMIN.Domain.Entities.Taller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class CotizacionReparacionConfiguration : IEntityTypeConfiguration<CotizacionReparacion>
{
    public void Configure(EntityTypeBuilder<CotizacionReparacion> builder)
    {
        builder.ToTable("cotizaciones_reparacion", t => t.HasCheckConstraint(
            "ck_cotizaciones_reparacion_decision_cliente",
            "decision_cliente IN ('Aprobada','Rechazada') OR decision_cliente IS NULL"));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.MontoEstimado).HasPrecision(12, 2).IsRequired();
        builder.Property(c => c.Fecha).IsRequired();

        builder.Property(c => c.DecisionCliente)
            .HasConversion<string?>()
            .HasMaxLength(20);

        builder.Property(c => c.CobroDiagnosticoRechazo).HasPrecision(12, 2);
        builder.Property(c => c.EvidenciaAprobacion).HasMaxLength(255);

        builder.HasIndex(c => c.OrdenTrabajoId).IsUnique();
    }
}
