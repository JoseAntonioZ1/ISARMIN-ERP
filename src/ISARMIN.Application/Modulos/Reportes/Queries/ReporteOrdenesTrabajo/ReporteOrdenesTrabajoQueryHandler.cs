using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Reportes.DTOs;
using ISARMIN.Application.Modulos.Taller.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.Queries.ReporteOrdenesTrabajo;

public class ReporteOrdenesTrabajoQueryHandler : IQueryHandler<ReporteOrdenesTrabajoQuery, ReporteOrdenesTrabajoDto>
{
    private readonly IReporteRepository _reporteRepository;

    public ReporteOrdenesTrabajoQueryHandler(IReporteRepository reporteRepository)
    {
        _reporteRepository = reporteRepository;
    }

    public async Task<ReporteOrdenesTrabajoDto> ManejarAsync(ReporteOrdenesTrabajoQuery consulta, CancellationToken cancellationToken = default)
    {
        var ordenes = await _reporteRepository.ObtenerOrdenesTrabajoAsync(consulta.Estado, consulta.Desde, consulta.Hasta, cancellationToken);

        return new ReporteOrdenesTrabajoDto(consulta.Desde, consulta.Hasta, ordenes.Count, ordenes.Select(o => o.ADto()).ToList());
    }
}
