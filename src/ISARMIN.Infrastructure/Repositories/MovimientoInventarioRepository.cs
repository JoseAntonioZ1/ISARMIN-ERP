using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class MovimientoInventarioRepository : IMovimientoInventarioRepository
{
    private readonly IsarminDbContext _dbContext;

    public MovimientoInventarioRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<MovimientoInventario>> ListarPorProductoAsync(
        Guid productoId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.MovimientosInventario.Where(m => m.ProductoId == productoId);

        if (desde is { } fechaDesde)
        {
            consulta = consulta.Where(m => m.Fecha >= fechaDesde);
        }

        if (hasta is { } fechaHasta)
        {
            consulta = consulta.Where(m => m.Fecha <= fechaHasta);
        }

        return await consulta.OrderByDescending(m => m.Fecha).ToListAsync(cancellationToken);
    }

    public void Agregar(MovimientoInventario movimiento) => _dbContext.MovimientosInventario.Add(movimiento);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
