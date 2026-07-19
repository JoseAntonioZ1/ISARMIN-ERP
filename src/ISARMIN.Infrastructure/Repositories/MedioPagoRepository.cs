using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Domain.Entities.Configuracion;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class MedioPagoRepository : IMedioPagoRepository
{
    private readonly IsarminDbContext _dbContext;

    public MedioPagoRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<MedioPago?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.MediosPago.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public Task<MedioPago?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        _dbContext.MediosPago.FirstOrDefaultAsync(m => m.Nombre == nombre, cancellationToken);

    public async Task<IReadOnlyCollection<MedioPago>> ListarTodosAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.MediosPago.OrderBy(m => m.Nombre).ToListAsync(cancellationToken);

    public void Agregar(MedioPago medioPago) => _dbContext.MediosPago.Add(medioPago);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
