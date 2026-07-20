using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Ventas;

public interface IVentaRepository
{
    Task<Venta?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Venta> Ventas, int Total)> BuscarAsync(
        EstadoVenta? estado, Guid? clienteId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default);

    void Agregar(Venta venta);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
