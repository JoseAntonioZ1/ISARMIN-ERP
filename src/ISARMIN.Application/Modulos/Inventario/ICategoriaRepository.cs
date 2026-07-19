using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario;

public interface ICategoriaRepository
{
    Task<Categoria?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Categoria?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);

    Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Categoria>> ListarTodasAsync(CancellationToken cancellationToken = default);

    void Agregar(Categoria categoria);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
