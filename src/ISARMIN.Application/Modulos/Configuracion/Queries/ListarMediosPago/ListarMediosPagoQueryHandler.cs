using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Configuracion.DTOs;

namespace ISARMIN.Application.Modulos.Configuracion.Queries.ListarMediosPago;

public class ListarMediosPagoQueryHandler : IQueryHandler<ListarMediosPagoQuery, IReadOnlyCollection<MedioPagoDto>>
{
    private readonly IMedioPagoRepository _medioPagoRepository;

    public ListarMediosPagoQueryHandler(IMedioPagoRepository medioPagoRepository)
    {
        _medioPagoRepository = medioPagoRepository;
    }

    public async Task<IReadOnlyCollection<MedioPagoDto>> ManejarAsync(ListarMediosPagoQuery consulta, CancellationToken cancellationToken = default)
    {
        var mediosPago = await _medioPagoRepository.ListarTodosAsync(cancellationToken);
        return mediosPago.Select(m => m.ADto()).ToList();
    }
}
