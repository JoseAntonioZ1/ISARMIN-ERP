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
            .OrderBy(r => r.Nombre)
            .ToListAsync(cancellationToken);
}
