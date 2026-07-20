using ISARMIN.Domain.Entities.Configuracion;
using ISARMIN.Domain.Entities.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class PagoVentaConfiguration : IEntityTypeConfiguration<PagoVenta>
{
    public void Configure(EntityTypeBuilder<PagoVenta> builder)
    {
        builder.ToTable("pagos_venta", t => t.HasCheckConstraint(
            "ck_pagos_venta_monto",
            "monto > 0"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Monto).HasPrecision(12, 2).IsRequired();

        builder.HasOne<Venta>()
            .WithMany(v => v.Pagos)
            .HasForeignKey(p => p.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<MedioPago>()
            .WithMany()
            .HasForeignKey(p => p.MedioPagoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
