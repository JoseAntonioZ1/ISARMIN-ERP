using ISARMIN.Domain.Entities.Configuracion;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.ServiciosCampo;
using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class ServicioCampoConfiguration : IEntityTypeConfiguration<ServicioCampo>
{
    public void Configure(EntityTypeBuilder<ServicioCampo> builder)
    {
        builder.ToTable("servicios_campo", t => t.HasCheckConstraint(
            "ck_servicios_campo_estado",
            "estado IN ('Solicitado','Agendado','EnEjecucion','Cerrado')"));

        builder.HasKey(s => s.Id);

        builder.Property(s => s.DescripcionTrabajo).HasColumnType("text").IsRequired();
        builder.Property(s => s.FechaSolicitud).IsRequired();

        builder.Property(s => s.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoServicioCampo.Solicitado)
            .IsRequired();

        builder.Property(s => s.MontoEstimado).HasPrecision(12, 2);
        builder.Property(s => s.EstadoFinal).HasMaxLength(50);
        builder.Property(s => s.Observaciones).HasColumnType("text");
        builder.Property(s => s.MontoPagado).HasPrecision(12, 2);
        builder.Property(s => s.SaldoPendiente).HasPrecision(12, 2);

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(s => s.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(s => s.TecnicoAsignadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(s => s.UsuarioCierreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(s => s.UsuarioAutorizoSaldoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<MedioPago>()
            .WithMany()
            .HasForeignKey(s => s.MedioPagoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(s => s.Detalles)
            .HasField("_detalles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
