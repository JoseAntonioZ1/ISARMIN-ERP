using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Application.Modulos.Reportes.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.Queries.ReporteInventario;

public class ReporteInventarioQueryHandler : IQueryHandler<ReporteInventarioQuery, ReporteInventarioDto>
{
    private readonly IReporteRepository _reporteRepository;

    public ReporteInventarioQueryHandler(IReporteRepository reporteRepository)
    {
        _reporteRepository = reporteRepository;
    }

    public async Task<ReporteInventarioDto> ManejarAsync(ReporteInventarioQuery consulta, CancellationToken cancellationToken = default)
    {
        var productos = await _reporteRepository.ObtenerProductosAsync(cancellationToken);
        var enQuiebre = productos.Where(p => p.StockMinimo is { } minimo && p.StockActual <= minimo);

        return new ReporteInventarioDto(productos.Select(p => p.ADto()).ToList(), enQuiebre.Select(p => p.ADto()).ToList());
    }
}
