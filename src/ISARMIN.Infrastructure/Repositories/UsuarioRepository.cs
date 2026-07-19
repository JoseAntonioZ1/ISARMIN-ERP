using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IsarminDbContext _dbContext;

    public UsuarioRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario, CancellationToken cancellationToken = default) =>
        _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario, cancellationToken);

    public Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Usuarios
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<Usuario> Usuarios, int Total)> ListarAsync(
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Usuarios
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Rol)
            .OrderBy(u => u.Nombre)
            .AsSplitQuery();

        var total = await consulta.CountAsync(cancellationToken);

        var usuarios = await consulta
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (usuarios, total);
    }

    public async Task<IReadOnlyCollection<string>> ObtenerPermisosEfectivosAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var permisos = await _dbContext.UsuarioRoles
            .Where(ur => ur.UsuarioId == usuarioId)
            .SelectMany(ur => ur.Rol.Permisos)
            .Select(p => new { p.Modulo, p.Accion })
            .Distinct()
            .ToListAsync(cancellationToken);

        return permisos
            .Select(p => $"{p.Modulo}.{p.Accion}")
            .ToList();
    }

    public void Agregar(Usuario usuario) => _dbContext.Usuarios.Add(usuario);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
