using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario;

public interface IMovimientoInventarioRepository
{
    Task<IReadOnlyCollection<MovimientoInventario>> ListarPorProductoAsync(
        Guid productoId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);

    void Agregar(MovimientoInventario movimiento);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
