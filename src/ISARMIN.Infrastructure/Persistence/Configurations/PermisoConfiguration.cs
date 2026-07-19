using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("permisos", t => t.HasCheckConstraint(
            "ck_permisos_accion",
            "accion IN ('Crear','Editar','Eliminar','Consultar','Anular')"));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Modulo)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Accion)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(p => new { p.RolId, p.Modulo, p.Accion }).IsUnique();

        builder.HasOne<Rol>()
            .WithMany(r => r.Permisos)
            .HasForeignKey(p => p.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(p => p.NombreLogico);

        // Permisos del Administrador para el módulo Usuarios (RF-010), sembrados aquí porque
        // el módulo Roles y Permisos (UC-04) todavía no existe para asignarlos desde la UI.
        // "Roles.Consultar" habilita el selector de roles del formulario de Usuarios.
        var rolAdministradorId = RolConfiguration.RolAdministradorId;
        builder.HasData(
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000b1"), RolId = rolAdministradorId, Modulo = "Usuarios", Accion = AccionPermiso.Crear },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000b2"), RolId = rolAdministradorId, Modulo = "Usuarios", Accion = AccionPermiso.Editar },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000b3"), RolId = rolAdministradorId, Modulo = "Usuarios", Accion = AccionPermiso.Eliminar },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000b4"), RolId = rolAdministradorId, Modulo = "Usuarios", Accion = AccionPermiso.Consultar },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000b5"), RolId = rolAdministradorId, Modulo = "Roles", Accion = AccionPermiso.Consultar },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000b6"), RolId = rolAdministradorId, Modulo = "Roles", Accion = AccionPermiso.Crear },
            new { Id = Guid.Parse("00000000-0000-0000-0000-0000000000b7"), RolId = rolAdministradorId, Modulo = "Roles", Accion = AccionPermiso.Editar });
    }
}
