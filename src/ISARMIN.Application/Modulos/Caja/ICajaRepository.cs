using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;

namespace ISARMIN.Application.Modulos.Caja;

public interface ICajaRepository
{
    Task<CajaEntity?> ObtenerAbiertaAsync(CancellationToken cancellationToken = default);

    Task<CajaEntity?> ObtenerMasRecienteAsync(CancellationToken cancellationToken = default);

    Task<CajaEntity?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Agregar(CajaEntity caja);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
