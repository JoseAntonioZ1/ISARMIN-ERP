using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Ventas;

/// <summary>UC-14 — Registrar Venta (RF-038 a RF-041, RF-089). RN-003/RN-006 a RN-008: el stock se
/// valida y descuenta al confirmar, sin reserva previa (responsabilidad del handler, no de este
/// agregado). RN-031: saldo pendiente siempre requiere autorización de un usuario
/// Administrador/Propietario. Origen fijo en <see cref="OrigenVenta.Directa"/> — vincular la venta a
/// una OT/Servicio de Campo (RF-083) queda fuera de alcance.</summary>
public class Venta : Entity
{
    public Guid? ClienteId { get; private set; }
    public TipoComprobante TipoComprobante { get; private set; }
    public OrigenVenta Origen { get; private set; } = OrigenVenta.Directa;
    public DateTime Fecha { get; private set; }
    public Guid UsuarioId { get; private set; }
    public decimal Total { get; private set; }
    public EstadoVenta Estado { get; private set; }

    public decimal? SaldoPendiente { get; private set; }
    public Guid? UsuarioAutorizoSaldoId { get; private set; }

    public string? MotivoAnulacion { get; private set; }
    public Guid? UsuarioAnuloId { get; private set; }
    public DateTime? FechaAnulacion { get; private set; }

    private readonly List<VentaDetalle> _detalles = [];
    public IReadOnlyCollection<VentaDetalle> Detalles => _detalles.AsReadOnly();

    private readonly List<PagoVenta> _pagos = [];
    public IReadOnlyCollection<PagoVenta> Pagos => _pagos.AsReadOnly();

    private Venta() { }

    public Venta(
        Guid? clienteId,
        TipoComprobante tipoComprobante,
        Guid usuarioId,
        DateTime fecha,
        IEnumerable<(Guid ProductoId, decimal Cantidad, decimal PrecioUnitario)> detalles,
        IEnumerable<(Guid MedioPagoId, decimal Monto)> pagos,
        Guid? usuarioAutorizoSaldoId)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("El usuario que registra la venta es obligatorio.", nameof(usuarioId));
        }

        var listaDetalles = detalles.ToList();
        if (listaDetalles.Count == 0)
        {
            throw new ArgumentException("La venta debe tener al menos un producto.", nameof(detalles));
        }

        ClienteId = clienteId;
        TipoComprobante = tipoComprobante;
        UsuarioId = usuarioId;
        Fecha = fecha;

        foreach (var (productoId, cantidad, precioUnitario) in listaDetalles)
        {
            _detalles.Add(new VentaDetalle(Id, productoId, cantidad, precioUnitario));
        }

        foreach (var (medioPagoId, monto) in pagos)
        {
            _pagos.Add(new PagoVenta(Id, medioPagoId, monto));
        }

        Total = _detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
        var montoPagado = _pagos.Sum(p => p.Monto);
        var saldoPendiente = Total - montoPagado;

        if (saldoPendiente < 0)
        {
            throw new ArgumentException("El monto pagado no puede exceder el total de la venta.", nameof(pagos));
        }

        if (saldoPendiente > 0)
        {
            if (usuarioAutorizoSaldoId is null)
            {
                throw new ArgumentException("Se requiere el usuario Administrador/Propietario que autorizó el saldo pendiente.", nameof(usuarioAutorizoSaldoId));
            }

            SaldoPendiente = saldoPendiente;
            UsuarioAutorizoSaldoId = usuarioAutorizoSaldoId;
            Estado = EstadoVenta.Registrada;
        }
        else
        {
            Estado = EstadoVenta.Pagada;
        }
    }

    public void Anular(string motivo, Guid usuarioAnuloId, DateTime fechaAnulacion)
    {
        if (Estado == EstadoVenta.Anulada)
        {
            throw new InvalidOperationException("La venta ya se encuentra anulada.");
        }

        if (string.IsNullOrWhiteSpace(motivo))
        {
            throw new ArgumentException("El motivo de anulación es obligatorio.", nameof(motivo));
        }

        Estado = EstadoVenta.Anulada;
        MotivoAnulacion = motivo;
        UsuarioAnuloId = usuarioAnuloId;
        FechaAnulacion = fechaAnulacion;
    }
}
