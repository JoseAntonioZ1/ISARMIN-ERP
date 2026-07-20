using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("ventas", t =>
        {
            t.HasCheckConstraint(
                "ck_ventas_tipo_comprobante",
                "tipo_comprobante IN ('Cotizacion','Boleta','Factura','NotaVenta','Ticket')");
            t.HasCheckConstraint(
                "ck_ventas_origen",
                "origen IN ('Directa','OrdenTrabajo','ServicioCampo')");
            t.HasCheckConstraint(
                "ck_ventas_estado",
                "estado IN ('Registrada','Emitida','Pagada','Anulada')");
        });

        builder.HasKey(v => v.Id);

        builder.Property(v => v.TipoComprobante)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(v => v.Origen)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(OrigenVenta.Directa)
            .IsRequired();

        builder.Property(v => v.Fecha).IsRequired();

        builder.Property(v => v.Total)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(v => v.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoVenta.Registrada)
            .IsRequired();

        builder.Property(v => v.SaldoPendiente).HasPrecision(12, 2);
        builder.Property(v => v.MotivoAnulacion).HasColumnType("text");

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(v => v.UsuarioAutorizoSaldoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(v => v.UsuarioAnuloId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(v => v.Detalles)
            .HasField("_detalles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(v => v.Pagos)
            .HasField("_pagos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
