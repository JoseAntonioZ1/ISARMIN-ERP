using ISARMIN.Domain.Entities.Compras;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Entities.Terceros;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class CompraConfiguration : IEntityTypeConfiguration<Compra>
{
    public void Configure(EntityTypeBuilder<Compra> builder)
    {
        builder.ToTable("compras");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Fecha)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(c => c.DocumentoCompraTipo)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(c => c.DocumentoCompraNumero)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Total)
            .HasPrecision(12, 2)
            .HasDefaultValue(0m)
            .IsRequired();

        builder.Navigation(c => c.Detalles)
            .HasField("_detalles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne<Proveedor>()
            .WithMany()
            .HasForeignKey(c => c.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
