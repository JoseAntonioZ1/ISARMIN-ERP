using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class OrdenTrabajoConfiguration : IEntityTypeConfiguration<OrdenTrabajo>
{
    public void Configure(EntityTypeBuilder<OrdenTrabajo> builder)
    {
        builder.ToTable("ordenes_trabajo", t =>
        {
            t.HasCheckConstraint(
                "ck_ordenes_trabajo_estado",
                "estado IN ('Recibido','Diagnosticado','Cotizado','Aprobado','Rechazado','EnReparacion','EnPruebas','ListoParaEntrega','Entregado')");
            t.HasCheckConstraint(
                "ck_ordenes_trabajo_estado_pago",
                "estado_pago IN ('CompletoAntes','CompletoAlMomento','Adelanto','SaldoPendiente') OR estado_pago IS NULL");
        });

        builder.HasKey(o => o.Id);

        builder.Property(o => o.EquipoDescripcion).HasMaxLength(255).IsRequired();
        builder.Property(o => o.FallaReportada).HasColumnType("text").IsRequired();
        builder.Property(o => o.FechaRecepcion).IsRequired();

        builder.Property(o => o.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoOrdenTrabajo.Recibido)
            .IsRequired();

        builder.Property(o => o.EstadoPago)
            .HasConversion<string?>()
            .HasMaxLength(20);

        builder.Property(o => o.MontoPagado).HasPrecision(12, 2);
        builder.Property(o => o.SaldoPendiente).HasPrecision(12, 2);
        builder.Property(o => o.ResultadoPruebas).HasColumnType("text");

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(o => o.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(o => o.UsuarioRecepcionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(o => o.UsuarioEntregaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(o => o.UsuarioAutorizoSaldoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Diagnostico)
            .WithOne()
            .HasForeignKey<Diagnostico>(d => d.OrdenTrabajoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.CotizacionReparacion)
            .WithOne()
            .HasForeignKey<CotizacionReparacion>(c => c.OrdenTrabajoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.ConsumosRepuesto)
            .HasField("_consumosRepuesto")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
