using ISARMIN.Domain.Entities.Identidad;
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
    }
}
