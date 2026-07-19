using ISARMIN.Domain.Entities.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class UsuarioRolConfiguration : IEntityTypeConfiguration<UsuarioRol>
{
    public void Configure(EntityTypeBuilder<UsuarioRol> builder)
    {
        builder.ToTable("usuario_rol");

        builder.HasKey(ur => new { ur.UsuarioId, ur.RolId });

        builder.HasOne(ur => ur.Rol)
            .WithMany()
            .HasForeignKey(ur => ur.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Usuario>()
            .WithMany(u => u.Roles)
            .HasForeignKey(ur => ur.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(new
        {
            UsuarioId = UsuarioConfiguration.UsuarioAdministradorSembradoId,
            RolId = RolConfiguration.RolAdministradorId,
        });
    }
}
