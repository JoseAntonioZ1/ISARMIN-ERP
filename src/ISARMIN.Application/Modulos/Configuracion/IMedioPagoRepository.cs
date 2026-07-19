using ISARMIN.Domain.Entities.Configuracion;

namespace ISARMIN.Application.Modulos.Configuracion;

public interface IMedioPagoRepository
{
    Task<MedioPago?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<MedioPago?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<MedioPago>> ListarTodosAsync(CancellationToken cancellationToken = default);

    void Agregar(MedioPago medioPago);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
