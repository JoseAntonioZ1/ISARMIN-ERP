using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class RolRepository : IRolRepository
{
    private readonly IsarminDbContext _dbContext;

    public RolRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Guid>> ObtenerIdsExistentesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idsSolicitados = ids.ToList();
        return await _dbContext.Roles
            .Where(r => idsSolicitados.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Rol>> ListarTodosAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Roles
            .Include(r => r.Permisos)
            .OrderBy(r => r.Nombre)
            .ToListAsync(cancellationToken);

    public Task<Rol?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Roles
            .Include(r => r.Permisos)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<Rol?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        _dbContext.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre, cancellationToken);

    public void Agregar(Rol rol) => _dbContext.Roles.Add(rol);

    public void AgregarPermisos(IEnumerable<Permiso> permisos) => _dbContext.Permisos.AddRange(permisos);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
