using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Caja.DTOs;
using ISARMIN.Application.Modulos.Reportes.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Reportes.Queries.ReporteCaja;

public class ReporteCajaQueryHandler : IQueryHandler<ReporteCajaQuery, ReporteCajaDto>
{
    private readonly IReporteRepository _reporteRepository;

    public ReporteCajaQueryHandler(IReporteRepository reporteRepository)
    {
        _reporteRepository = reporteRepository;
    }

    public async Task<ReporteCajaDto> ManejarAsync(ReporteCajaQuery consulta, CancellationToken cancellationToken = default)
    {
        var (cajas, movimientos) = await _reporteRepository.ObtenerCajaAsync(consulta.Desde, consulta.Hasta, cancellationToken);

        var totalIngresos = movimientos.Where(m => m.Tipo == TipoMovimientoCaja.Ingreso).Sum(m => m.Monto);
        var totalEgresos = movimientos.Where(m => m.Tipo == TipoMovimientoCaja.Egreso).Sum(m => m.Monto);

        return new ReporteCajaDto(
            consulta.Desde, consulta.Hasta, totalIngresos, totalEgresos, totalIngresos - totalEgresos,
            cajas.Select(c => c.ADto()).ToList(), movimientos.Select(m => m.ADto()).ToList());
    }
}
