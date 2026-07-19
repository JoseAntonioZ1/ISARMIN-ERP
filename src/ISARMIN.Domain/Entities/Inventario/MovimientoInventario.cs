using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Inventario;

/// <summary>Kardex (RN-002, RF-027) — registro histórico de todo movimiento de inventario.
/// Referencia genérica a su origen (ADR-012, Architecture-Overview.md §7.3): solo
/// <see cref="ProductoId"/> es FK fuerte; <see cref="OrigenTipo"/>/<see cref="OrigenId"/> son
/// informativos, sin integridad declarativa, porque el Kardex es en esencia un log histórico.
/// Por ahora solo existe la fábrica <see cref="CrearAjuste"/> (UC-12): los demás tipos de
/// movimiento (Compra, Venta, ConsumoTaller, ConsumoCampo, Devolucion) los generarán sus
/// respectivos módulos (Compras, Ventas, Taller, Servicios de Campo) cuando existan.</summary>
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
}
