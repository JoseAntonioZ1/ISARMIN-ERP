using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Taller.DTOs;

namespace ISARMIN.Application.Modulos.Taller.Queries.ObtenerOrdenTrabajo;

public class ObtenerOrdenTrabajoQueryHandler : IQueryHandler<ObtenerOrdenTrabajoQuery, OrdenTrabajoDetalleDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IGarantiaRepository _garantiaRepository;

    public ObtenerOrdenTrabajoQueryHandler(IOrdenTrabajoRepository ordenTrabajoRepository, IGarantiaRepository garantiaRepository)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _garantiaRepository = garantiaRepository;
    }

    public async Task<OrdenTrabajoDetalleDto> ManejarAsync(ObtenerOrdenTrabajoQuery consulta, CancellationToken cancellationToken = default)
    {
        var ot = await _ordenTrabajoRepository.ObtenerPorIdAsync(consulta.Id, cancellationToken)
            ?? throw new OrdenTrabajoNoEncontradaException(consulta.Id);

        var garantia = await _garantiaRepository.ObtenerPorOrdenTrabajoIdAsync(ot.Id, cancellationToken);

        return new OrdenTrabajoDetalleDto(ot.ADto(), garantia?.ADto());
    }
}
