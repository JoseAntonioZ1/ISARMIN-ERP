using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario;

public interface IProductoRepository
{
    Task<Producto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Producto?> ObtenerPorCodigoInternoAsync(string codigoInterno, CancellationToken cancellationToken = default);

    Task<Producto?> ObtenerPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Producto> Productos, int Total)> BuscarAsync(
        string? termino, Guid? categoriaId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    void Agregar(Producto producto);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
