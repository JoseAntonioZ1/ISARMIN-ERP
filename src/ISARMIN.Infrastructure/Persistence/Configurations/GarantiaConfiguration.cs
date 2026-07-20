using ISARMIN.Domain.Entities.Taller;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class GarantiaConfiguration : IEntityTypeConfiguration<Garantia>
{
    public void Configure(EntityTypeBuilder<Garantia> builder)
    {
        builder.ToTable("garantias", t => t.HasCheckConstraint(
            "ck_garantias_fecha_fin",
            "fecha_fin > fecha_inicio"));

        builder.HasKey(g => g.Id);

        builder.Property(g => g.FechaInicio).HasColumnType("date").IsRequired();
        builder.Property(g => g.FechaFin).HasColumnType("date").IsRequired();

        builder.HasOne<OrdenTrabajo>()
            .WithMany()
            .HasForeignKey(g => g.OrdenTrabajoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OrdenTrabajo>()
            .WithMany()
            .HasForeignKey(g => g.OrdenTrabajoReingresoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
