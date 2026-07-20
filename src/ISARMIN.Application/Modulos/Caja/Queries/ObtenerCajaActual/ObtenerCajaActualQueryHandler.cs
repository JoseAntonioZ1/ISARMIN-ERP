using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Caja.DTOs;

namespace ISARMIN.Application.Modulos.Caja.Queries.ObtenerCajaActual;

/// <summary>Devuelve la caja abierta actual o, si no hay ninguna abierta, la más reciente (cerrada)
/// para que la UI pueda mostrar el último cierre antes de una nueva apertura. Null si nunca se abrió una caja.</summary>
public class ObtenerCajaActualQueryHandler : IQueryHandler<ObtenerCajaActualQuery, CajaDto?>
{
    private readonly ICajaRepository _cajaRepository;

    public ObtenerCajaActualQueryHandler(ICajaRepository cajaRepository)
    {
        _cajaRepository = cajaRepository;
    }

    public async Task<CajaDto?> ManejarAsync(ObtenerCajaActualQuery consulta, CancellationToken cancellationToken = default)
    {
        var caja = await _cajaRepository.ObtenerAbiertaAsync(cancellationToken)
            ?? await _cajaRepository.ObtenerMasRecienteAsync(cancellationToken);

        return caja?.ADto();
    }
}
