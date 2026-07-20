using ISARMIN.Application.Modulos.Reportes;
using ISARMIN.Domain.Entities.Caja;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.ServiciosCampo;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Infrastructure.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly IsarminDbContext _dbContext;

    public ReporteRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Venta>> ObtenerVentasAsync(
        DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Ventas.Include(v => v.Detalles).Include(v => v.Pagos).AsQueryable();

        if (desde is { } desdeValor)
        {
            consulta = consulta.Where(v => v.Fecha >= desdeValor);
        }

        if (hasta is { } hastaValor)
        {
            consulta = consulta.Where(v => v.Fecha <= hastaValor);
        }

        return await consulta.OrderByDescending(v => v.Fecha).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Producto>> ObtenerProductosAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Productos.OrderBy(p => p.Nombre).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<OrdenTrabajo>> ObtenerOrdenesTrabajoAsync(
        EstadoOrdenTrabajo? estado, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.OrdenesTrabajo
            .Include(o => o.Diagnostico)
            .Include(o => o.CotizacionReparacion)
            .Include(o => o.ConsumosRepuesto)
            .AsQueryable();

        if (estado is { } estadoValor)
        {
            consulta = consulta.Where(o => o.Estado == estadoValor);
        }

        if (desde is { } desdeValor)
        {
            consulta = consulta.Where(o => o.FechaRecepcion >= desdeValor);
        }

        if (hasta is { } hastaValor)
        {
            consulta = consulta.Where(o => o.FechaRecepcion <= hastaValor);
        }

        return await consulta.OrderByDescending(o => o.FechaRecepcion).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ServicioCampo>> ObtenerServiciosCampoAsync(
        Guid? tecnicoAsignadoId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.ServiciosCampo.Include(s => s.Detalles).AsQueryable();

        if (tecnicoAsignadoId is { } tecnicoId)
        {
            consulta = consulta.Where(s => s.TecnicoAsignadoId == tecnicoId);
        }

        if (desde is { } desdeValor)
        {
            consulta = consulta.Where(s => s.FechaSolicitud >= desdeValor);
        }

        if (hasta is { } hastaValor)
        {
            consulta = consulta.Where(s => s.FechaSolicitud <= hastaValor);
        }

        return await consulta.OrderByDescending(s => s.FechaSolicitud).ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyCollection<CajaEntity> Cajas, IReadOnlyCollection<MovimientoCaja> Movimientos)> ObtenerCajaAsync(
        DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var consultaCajas = _dbContext.Cajas.AsQueryable();
        var consultaMovimientos = _dbContext.MovimientosCaja.AsQueryable();

        if (desde is { } desdeValor)
        {
            consultaCajas = consultaCajas.Where(c => c.FechaApertura >= desdeValor);
            consultaMovimientos = consultaMovimientos.Where(m => m.Fecha >= desdeValor);
        }

        if (hasta is { } hastaValor)
        {
            consultaCajas = consultaCajas.Where(c => c.FechaApertura <= hastaValor);
            consultaMovimientos = consultaMovimientos.Where(m => m.Fecha <= hastaValor);
        }

        var cajas = await consultaCajas.OrderByDescending(c => c.FechaApertura).ToListAsync(cancellationToken);
        var movimientos = await consultaMovimientos.OrderByDescending(m => m.Fecha).ToListAsync(cancellationToken);

        return (cajas, movimientos);
    }
}
