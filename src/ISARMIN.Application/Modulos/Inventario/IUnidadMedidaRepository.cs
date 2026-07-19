using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario;

public interface IUnidadMedidaRepository
{
    Task<UnidadMedida?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UnidadMedida?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);

    Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UnidadMedida>> ListarTodasAsync(CancellationToken cancellationToken = default);

    void Agregar(UnidadMedida unidadMedida);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
