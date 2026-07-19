using ISARMIN.Domain.Entities.Identidad;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Persistence;

public class IsarminDbContext : DbContext
{
    public IsarminDbContext(DbContextOptions<IsarminDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IsarminDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
