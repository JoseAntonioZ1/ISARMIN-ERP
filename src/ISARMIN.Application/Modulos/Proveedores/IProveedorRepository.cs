using ISARMIN.Domain.Entities.Terceros;

namespace ISARMIN.Application.Modulos.Proveedores;

public interface IProveedorRepository
{
    Task<Proveedor?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Proveedor> Proveedores, int Total)> BuscarAsync(
        string? termino, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    void Agregar(Proveedor proveedor);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
