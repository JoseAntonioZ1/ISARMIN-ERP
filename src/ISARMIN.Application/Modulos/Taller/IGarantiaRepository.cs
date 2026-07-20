using ISARMIN.Domain.Entities.Taller;

namespace ISARMIN.Application.Modulos.Taller;

public interface IGarantiaRepository
{
    Task<Garantia?> ObtenerPorOrdenTrabajoIdAsync(Guid ordenTrabajoId, CancellationToken cancellationToken = default);

    void Agregar(Garantia garantia);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
