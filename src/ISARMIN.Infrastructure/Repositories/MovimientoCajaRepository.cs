using ISARMIN.Application.Modulos.Caja;
using ISARMIN.Domain.Enums;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MovimientoCajaEntity = ISARMIN.Domain.Entities.Caja.MovimientoCaja;

namespace ISARMIN.Infrastructure.Repositories;

public class MovimientoCajaRepository : IMovimientoCajaRepository
{
    private readonly IsarminDbContext _dbContext;

    public MovimientoCajaRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<MovimientoCajaEntity>> ListarPorCajaAsync(
        Guid cajaId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.MovimientosCaja.Where(m => m.CajaId == cajaId);

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

    public async Task<(decimal Ingresos, decimal Egresos)> ObtenerTotalesAsync(Guid cajaId, CancellationToken cancellationToken = default)
    {
        var movimientos = _dbContext.MovimientosCaja.Where(m => m.CajaId == cajaId);

        var ingresos = await movimientos.Where(m => m.Tipo == TipoMovimientoCaja.Ingreso).SumAsync(m => (decimal?)m.Monto, cancellationToken) ?? 0m;
        var egresos = await movimientos.Where(m => m.Tipo == TipoMovimientoCaja.Egreso).SumAsync(m => (decimal?)m.Monto, cancellationToken) ?? 0m;

        return (ingresos, egresos);
    }

    public void Agregar(MovimientoCajaEntity movimiento) => _dbContext.MovimientosCaja.Add(movimiento);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
