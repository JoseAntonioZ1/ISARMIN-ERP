using ISARMIN.Domain.Entities.Caja;

namespace ISARMIN.Application.Modulos.Caja;

public interface IMovimientoCajaRepository
{
    Task<IReadOnlyCollection<MovimientoCaja>> ListarPorCajaAsync(
        Guid cajaId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);

    Task<(decimal Ingresos, decimal Egresos)> ObtenerTotalesAsync(Guid cajaId, CancellationToken cancellationToken = default);

    void Agregar(MovimientoCaja movimiento);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
