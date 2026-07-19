using ISARMIN.Domain.Entities.Compras;

namespace ISARMIN.Application.Modulos.Compras;

public interface ICompraRepository
{
    Task<Compra?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Compra> Compras, int Total)> BuscarAsync(
        Guid? proveedorId, Guid? productoId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    void Agregar(Compra compra);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
