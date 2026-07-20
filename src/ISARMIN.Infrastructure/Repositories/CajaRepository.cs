using ISARMIN.Application.Modulos.Caja;
using ISARMIN.Domain.Enums;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Infrastructure.Repositories;

public class CajaRepository : ICajaRepository
{
    private readonly IsarminDbContext _dbContext;

    public CajaRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CajaEntity?> ObtenerAbiertaAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Cajas.FirstOrDefaultAsync(c => c.Estado == EstadoCaja.Abierta, cancellationToken);

    public Task<CajaEntity?> ObtenerMasRecienteAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Cajas.OrderByDescending(c => c.FechaApertura).FirstOrDefaultAsync(cancellationToken);

    public Task<CajaEntity?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Cajas.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Agregar(CajaEntity caja) => _dbContext.Cajas.Add(caja);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
