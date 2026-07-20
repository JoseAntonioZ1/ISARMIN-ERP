using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Inventario;

/// <summary>Kardex (RN-002, RF-027) — registro histórico de todo movimiento de inventario.
/// Referencia genérica a su origen (ADR-012, Architecture-Overview.md §7.3): solo
/// <see cref="ProductoId"/> es FK fuerte; <see cref="OrigenTipo"/>/<see cref="OrigenId"/> son
/// informativos, sin integridad declarativa, porque el Kardex es en esencia un log histórico.
/// Fábricas existentes: <see cref="CrearAjuste"/> (UC-12), <see cref="CrearCompra"/> (UC-13),
/// <see cref="CrearConsumoTaller"/> (UC-25), <see cref="CrearConsumoCampo"/> (UC-32),
/// <see cref="CrearVenta"/> (UC-14), <see cref="CrearDevolucion"/> (UC-16/UC-17).</summary>
public class MovimientoInventario : Entity
{
    public Guid ProductoId { get; private set; }
    public TipoMovimientoInventario TipoMovimiento { get; private set; }
    public decimal Cantidad { get; private set; }
    public string? OrigenTipo { get; private set; }
    public Guid? OrigenId { get; private set; }
    public string? MotivoAjuste { get; private set; }
    public Guid UsuarioId { get; private set; }
    public DateTime Fecha { get; private set; }

    private MovimientoInventario() { }

    private MovimientoInventario(
        Guid productoId,
        TipoMovimientoInventario tipoMovimiento,
        decimal cantidad,
        string? origenTipo,
        Guid? origenId,
        string? motivoAjuste,
        Guid usuarioId,
        DateTime fecha)
    {
        ProductoId = productoId;
        TipoMovimiento = tipoMovimiento;
        Cantidad = cantidad;
        OrigenTipo = origenTipo;
        OrigenId = origenId;
        MotivoAjuste = motivoAjuste;
        UsuarioId = usuarioId;
        Fecha = fecha;
    }

    /// <summary>UC-12/RF-030 — ajuste manual exclusivo del Administrador, motivo obligatorio (RN-008).</summary>
    public static MovimientoInventario CrearAjuste(Guid productoId, decimal cantidadAjuste, string motivo, Guid usuarioId, DateTime fecha)
    {
        if (cantidadAjuste == 0)
        {
            throw new ArgumentException("El ajuste debe ser distinto de cero.", nameof(cantidadAjuste));
        }

        if (string.IsNullOrWhiteSpace(motivo))
        {
            throw new ArgumentException("El motivo del ajuste es obligatorio.", nameof(motivo));
        }

        return new MovimientoInventario(productoId, TipoMovimientoInventario.Ajuste, cantidadAjuste, null, null, motivo, usuarioId, fecha);
    }

    /// <summary>UC-13/RF-027 — movimiento de entrada generado automáticamente al registrar una Compra.</summary>
    public static MovimientoInventario CrearCompra(Guid productoId, decimal cantidad, Guid compraId, Guid usuarioId, DateTime fecha)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad comprada debe ser mayor a cero.", nameof(cantidad));
        }

        return new MovimientoInventario(productoId, TipoMovimientoInventario.Compra, cantidad, "Compra", compraId, null, usuarioId, fecha);
    }

    /// <summary>UC-25/RF-057 — movimiento de salida generado al registrar los repuestos consumidos en
    /// una reparación (RN-002, RN-006, RN-018).</summary>
    public static MovimientoInventario CrearConsumoTaller(Guid productoId, decimal cantidad, Guid ordenTrabajoId, Guid usuarioId, DateTime fecha)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad consumida debe ser mayor a cero.", nameof(cantidad));
        }

        return new MovimientoInventario(productoId, TipoMovimientoInventario.ConsumoTaller, -cantidad, "OrdenTrabajo", ordenTrabajoId, null, usuarioId, fecha);
    }

    /// <summary>UC-32/RF-067 — movimiento de salida generado al registrar los materiales consumidos
    /// en un servicio de campo (RN-002, RN-006, RN-020).</summary>
    public static MovimientoInventario CrearConsumoCampo(Guid productoId, decimal cantidad, Guid servicioCampoId, Guid usuarioId, DateTime fecha)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad consumida debe ser mayor a cero.", nameof(cantidad));
        }

        return new MovimientoInventario(productoId, TipoMovimientoInventario.ConsumoCampo, -cantidad, "ServicioCampo", servicioCampoId, null, usuarioId, fecha);
    }

    /// <summary>UC-14/RF-041 — movimiento de salida generado al confirmar una venta (RN-002, RN-003, RN-006).</summary>
    public static MovimientoInventario CrearVenta(Guid productoId, decimal cantidad, Guid ventaId, Guid usuarioId, DateTime fecha)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad vendida debe ser mayor a cero.", nameof(cantidad));
        }

        return new MovimientoInventario(productoId, TipoMovimientoInventario.Venta, -cantidad, "Venta", ventaId, null, usuarioId, fecha);
    }

    /// <summary>UC-16/RF-043 (reversión al anular) y UC-17/RF-091 (devolución de cliente) — movimiento
    /// de entrada que repone el stock, trazable a la venta original (RN-032).</summary>
    public static MovimientoInventario CrearDevolucion(Guid productoId, decimal cantidad, Guid ventaId, Guid usuarioId, DateTime fecha)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad devuelta debe ser mayor a cero.", nameof(cantidad));
        }

        return new MovimientoInventario(productoId, TipoMovimientoInventario.Devolucion, cantidad, "Venta", ventaId, null, usuarioId, fecha);
    }
}
