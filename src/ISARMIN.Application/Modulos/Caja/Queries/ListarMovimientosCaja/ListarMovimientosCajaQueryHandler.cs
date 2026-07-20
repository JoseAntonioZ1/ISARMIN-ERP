using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Caja.DTOs;

namespace ISARMIN.Application.Modulos.Caja.Queries.ListarMovimientosCaja;

/// <summary>UC-20 — historial de movimientos de una caja. Si no se indica <c>CajaId</c>, se usa la
/// caja abierta actual o, si no hay ninguna abierta, la más reciente (para revisar el último cierre).</summary>
public class ListarMovimientosCajaQueryHandler : IQueryHandler<ListarMovimientosCajaQuery, IReadOnlyCollection<MovimientoCajaDto>>
{
    private readonly ICajaRepository _cajaRepository;
    private readonly IMovimientoCajaRepository _movimientoCajaRepository;

    public ListarMovimientosCajaQueryHandler(ICajaRepository cajaRepository, IMovimientoCajaRepository movimientoCajaRepository)
    {
        _cajaRepository = cajaRepository;
        _movimientoCajaRepository = movimientoCajaRepository;
    }

    public async Task<IReadOnlyCollection<MovimientoCajaDto>> ManejarAsync(ListarMovimientosCajaQuery consulta, CancellationToken cancellationToken = default)
    {
        var cajaId = consulta.CajaId
            ?? (await _cajaRepository.ObtenerAbiertaAsync(cancellationToken))?.Id
            ?? (await _cajaRepository.ObtenerMasRecienteAsync(cancellationToken))?.Id;

        if (cajaId is null)
        {
            return [];
        }

        var movimientos = await _movimientoCajaRepository.ListarPorCajaAsync(cajaId.Value, consulta.Desde, consulta.Hasta, cancellationToken);
        return movimientos.Select(m => m.ADto()).ToList();
    }
}
