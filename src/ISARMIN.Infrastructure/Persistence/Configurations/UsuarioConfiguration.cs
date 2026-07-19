using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISARMIN.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    /// <summary>
    /// Usuario Administrador sembrado para poder iniciar sesión por primera vez (problema de arranque:
    /// crear un usuario requiere estar autenticado como Administrador, que aún no existe).
    /// Contraseña temporal documentada en README.md — debe cambiarse en cuanto exista el módulo de Usuarios.
    /// </summary>
    public static readonly Guid UsuarioAdministradorSembradoId = Guid.Parse("00000000-0000-0000-0000-0000000000a1");

    private const string CredencialHashSembrada =
        "AQAAAAIAAYagAAAAEFF/mQcjo7I1pcwzMaEjr6h0JjtxjfNsg4QMpRJbGRHzyMbbydhPHhidcXlHS51ubQ==";

    private static readonly DateTime FechaCreacionSembrada = new(2026, 7, 19, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios", t => t.HasCheckConstraint(
            "ck_usuarios_estado",
            "estado IN ('Activo','Inactivo')"));

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.NombreUsuario)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(u => u.NombreUsuario).IsUnique();

        builder.Property(u => u.CredencialHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.IntentosFallidos)
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(u => u.Estado)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(EstadoRegistro.Activo)
            .IsRequired();

        builder.Property(u => u.FechaCreacion)
            .IsRequired();

        builder.Navigation(u => u.Roles)
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasData(new
        {
            Id = UsuarioAdministradorSembradoId,
            Nombre = "Administrador ISARMIN",
            NombreUsuario = "admin",
            CredencialHash = CredencialHashSembrada,
            IntentosFallidos = 0,
            BloqueadoHasta = (DateTime?)null,
            Estado = EstadoRegistro.Activo,
            FechaCreacion = FechaCreacionSembrada,
        });
    }
}
