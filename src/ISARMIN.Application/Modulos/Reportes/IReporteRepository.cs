using CajaEntity = ISARMIN.Domain.Entities.Caja.Caja;
using ISARMIN.Domain.Entities.Caja;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.ServiciosCampo;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Reportes;

/// <summary>UC-35/RF-072 a RF-076 — capa de solo lectura para los reportes operativos; no forma
/// parte de los repositorios transaccionales de cada módulo porque sus consultas (sin paginar,
/// agregadas por rango de fechas) no se usan fuera de este caso de uso.</summary>
public interface IReporteRepository
{
    Task<IReadOnlyCollection<Venta>> ObtenerVentasAsync(DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Producto>> ObtenerProductosAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OrdenTrabajo>> ObtenerOrdenesTrabajoAsync(
        EstadoOrdenTrabajo? estado, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ServicioCampo>> ObtenerServiciosCampoAsync(
        Guid? tecnicoAsignadoId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<CajaEntity> Cajas, IReadOnlyCollection<MovimientoCaja> Movimientos)> ObtenerCajaAsync(
        DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);
}
