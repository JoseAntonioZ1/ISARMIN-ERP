using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Caja;

/// <summary>UC-20 — Registrar Movimiento de Caja (RF-047). Por ahora solo existe la fábrica
/// <see cref="Registrar"/> para movimientos manuales sin origen transaccional (RN-026: gasto
/// operativo, retiro del propietario o aporte de capital) — el endpoint que la usa está descrito en
/// API-Design.md como "egreso manual/gasto". Los movimientos ligados a Venta, Compra, Orden de
/// Trabajo, Servicio de Campo o Saldo Pendiente (Architecture-Overview.md §7.2, FK opcionales) se
/// agregarán cuando esos módulos existan, con sus propias fábricas y columnas de origen.</summary>
public class MovimientoCaja : Entity
{
    public Guid CajaId { get; private set; }
    public TipoMovimientoCaja Tipo { get; private set; }
    public decimal Monto { get; private set; }
    public ConceptoMovimientoCaja Concepto { get; private set; }
    public string? Descripcion { get; private set; }
    public Guid UsuarioId { get; private set; }
    public DateTime Fecha { get; private set; }

    private MovimientoCaja() { }

    private MovimientoCaja(
        Guid cajaId, TipoMovimientoCaja tipo, decimal monto, ConceptoMovimientoCaja concepto,
        string? descripcion, Guid usuarioId, DateTime fecha)
    {
        CajaId = cajaId;
        Tipo = tipo;
        Monto = monto;
        Concepto = concepto;
        Descripcion = descripcion;
        UsuarioId = usuarioId;
        Fecha = fecha;
    }

    /// <summary>RN-026 — el tipo (Ingreso/Egreso) se deriva del concepto, no se captura por separado:
    /// AporteCapital es siempre un ingreso; GastoOperativo y RetiroPropietario son siempre un egreso.</summary>
    public static MovimientoCaja Registrar(
        Guid cajaId, ConceptoMovimientoCaja concepto, decimal monto, string? descripcion, Guid usuarioId, DateTime fecha)
    {
        if (monto <= 0)
        {
            throw new ArgumentException("El monto debe ser mayor a cero.", nameof(monto));
        }

        var tipo = concepto == ConceptoMovimientoCaja.AporteCapital ? TipoMovimientoCaja.Ingreso : TipoMovimientoCaja.Egreso;

        return new MovimientoCaja(cajaId, tipo, monto, concepto, descripcion, usuarioId, fecha);
    }
}
