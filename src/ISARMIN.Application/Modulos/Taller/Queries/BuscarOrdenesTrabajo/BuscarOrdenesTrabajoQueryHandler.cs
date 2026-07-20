using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Taller.DTOs;

namespace ISARMIN.Application.Modulos.Taller.Queries.BuscarOrdenesTrabajo;

public class BuscarOrdenesTrabajoQueryHandler : IQueryHandler<BuscarOrdenesTrabajoQuery, ListadoPaginadoDto<OrdenTrabajoDto>>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;

    public BuscarOrdenesTrabajoQueryHandler(IOrdenTrabajoRepository ordenTrabajoRepository)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
    }

    public async Task<ListadoPaginadoDto<OrdenTrabajoDto>> ManejarAsync(BuscarOrdenesTrabajoQuery consulta, CancellationToken cancellationToken = default)
    {
        var (ordenesTrabajo, total) = await _ordenTrabajoRepository.BuscarAsync(
            consulta.Estado, consulta.ClienteId, consulta.Pagina, consulta.TamanoPagina, cancellationToken);

        return new ListadoPaginadoDto<OrdenTrabajoDto>(ordenesTrabajo.Select(ot => ot.ADto()).ToList(), total, consulta.Pagina, consulta.TamanoPagina);
    }
}
