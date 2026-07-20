using ISARMIN.Domain.Entities.Configuracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class ConfiguracionEmpresaConfiguration : IEntityTypeConfiguration<ConfiguracionEmpresa>
{
    /// <summary>Fila única sembrada — no existe un comando de creación (UC-37/RF-080).</summary>
    public static readonly Guid ConfiguracionEmpresaId = Guid.Parse("00000000-0000-0000-0000-0000000000e1");

    public void Configure(EntityTypeBuilder<ConfiguracionEmpresa> builder)
    {
        builder.ToTable("configuracion_empresa");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.RazonSocial).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Ruc).HasMaxLength(11);
        builder.Property(c => c.Direccion).HasColumnType("text");
        builder.Property(c => c.Logo).HasColumnType("text");
        builder.Property(c => c.MontoAperturaCajaPredeterminado).HasPrecision(12, 2);

        builder.HasData(new { Id = ConfiguracionEmpresaId, RazonSocial = "ISARMIN PERÚ S.A.C." });
    }
}
