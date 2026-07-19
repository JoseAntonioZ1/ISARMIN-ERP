using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class UnidadMedidaRepository : IUnidadMedidaRepository
{
    private readonly IsarminDbContext _dbContext;

    public UnidadMedidaRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UnidadMedida?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.UnidadesMedida.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<UnidadMedida?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        _dbContext.UnidadesMedida.FirstOrDefaultAsync(u => u.Nombre == nombre, cancellationToken);

    public Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.UnidadesMedida.AnyAsync(u => u.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<UnidadMedida>> ListarTodasAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.UnidadesMedida.OrderBy(u => u.Nombre).ToListAsync(cancellationToken);

    public void Agregar(UnidadMedida unidadMedida) => _dbContext.UnidadesMedida.Add(unidadMedida);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
