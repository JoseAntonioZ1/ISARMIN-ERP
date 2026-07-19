using ISARMIN.Domain.Entities.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    /// <summary>CAT-005 — los 3 roles reales confirmados por el propietario (dato semilla, no fijo en código).</summary>
    public static readonly Guid RolAdministradorId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid RolVentasId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static readonly Guid RolTecnicoId = Guid.Parse("00000000-0000-0000-0000-000000000003");

    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nombre)
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(r => r.Nombre).IsUnique();

        builder.Property(r => r.Descripcion);

        builder.Navigation(r => r.Permisos)
            .HasField("_permisos")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(
            new { Id = RolAdministradorId, Nombre = "Administrador", Descripcion = (string?)"Acceso completo — Propietario." },
            new { Id = RolVentasId, Nombre = "Ventas", Descripcion = (string?)"Atención al cliente, ventas, caja, consulta de inventario." },
            new { Id = RolTecnicoId, Nombre = "Técnico", Descripcion = (string?)"Taller, diagnósticos, órdenes de trabajo, servicios de campo." });
    }
}
