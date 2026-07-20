using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Reportes.DTOs;
using ISARMIN.Application.Modulos.Ventas.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Reportes.Queries.ReporteVentas;

public class ReporteVentasQueryHandler : IQueryHandler<ReporteVentasQuery, ReporteVentasDto>
{
    private readonly IReporteRepository _reporteRepository;

    public ReporteVentasQueryHandler(IReporteRepository reporteRepository)
    {
        _reporteRepository = reporteRepository;
    }

    public async Task<ReporteVentasDto> ManejarAsync(ReporteVentasQuery consulta, CancellationToken cancellationToken = default)
    {
        var ventas = await _reporteRepository.ObtenerVentasAsync(consulta.Desde, consulta.Hasta, cancellationToken);
        var montoTotal = ventas.Where(v => v.Estado != EstadoVenta.Anulada).Sum(v => v.Total);

        return new ReporteVentasDto(consulta.Desde, consulta.Hasta, ventas.Count, montoTotal, ventas.Select(v => v.ADto()).ToList());
    }
}
