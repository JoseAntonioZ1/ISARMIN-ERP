using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Reportes.DTOs;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;

namespace ISARMIN.Application.Modulos.Reportes.Queries.ReporteServiciosCampo;

public class ReporteServiciosCampoQueryHandler : IQueryHandler<ReporteServiciosCampoQuery, ReporteServiciosCampoDto>
{
    private readonly IReporteRepository _reporteRepository;

    public ReporteServiciosCampoQueryHandler(IReporteRepository reporteRepository)
    {
        _reporteRepository = reporteRepository;
    }

    public async Task<ReporteServiciosCampoDto> ManejarAsync(ReporteServiciosCampoQuery consulta, CancellationToken cancellationToken = default)
    {
        var servicios = await _reporteRepository.ObtenerServiciosCampoAsync(
            consulta.TecnicoAsignadoId, consulta.Desde, consulta.Hasta, cancellationToken);

        return new ReporteServiciosCampoDto(consulta.Desde, consulta.Hasta, servicios.Count, servicios.Select(s => s.ADto()).ToList());
    }
}
