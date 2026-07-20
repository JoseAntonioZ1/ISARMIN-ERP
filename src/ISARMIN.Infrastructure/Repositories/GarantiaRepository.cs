using ISARMIN.Application.Modulos.Taller;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class GarantiaRepository : IGarantiaRepository
{
    private readonly IsarminDbContext _dbContext;

    public GarantiaRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Garantia?> ObtenerPorOrdenTrabajoIdAsync(Guid ordenTrabajoId, CancellationToken cancellationToken = default) =>
        _dbContext.Garantias.FirstOrDefaultAsync(g => g.OrdenTrabajoId == ordenTrabajoId, cancellationToken);

    public void Agregar(Garantia garantia) => _dbContext.Garantias.Add(garantia);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
