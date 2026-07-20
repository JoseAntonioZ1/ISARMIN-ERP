using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Taller;

/// <summary>UC-22 a UC-26 — Orden de Trabajo (RF-051 a RF-060). Aggregate root que gobierna la
/// máquina de estados del Taller. <see cref="RegistrarReparacion"/> combina consumo de repuestos y
/// resultado de pruebas (RF-057, RF-058) en un solo paso, transicionando directo de Aprobado a
/// ListoParaEntrega — API-Design.md define un único endpoint para todo el paso de reparación, por lo
/// que EnReparacion/EnPruebas no se persisten como estados intermedios reales.</summary>
public class OrdenTrabajo : Entity
{
    public Guid ClienteId { get; private set; }
    public string EquipoDescripcion { get; private set; } = null!;
    public string FallaReportada { get; private set; } = null!;
    public DateTime FechaRecepcion { get; private set; }
    public Guid UsuarioRecepcionId { get; private set; }
    public EstadoOrdenTrabajo Estado { get; private set; }

    public DateTime? FechaEntrega { get; private set; }
    public Guid? UsuarioEntregaId { get; private set; }
    public EstadoPagoOrdenTrabajo? EstadoPago { get; private set; }
    public decimal? MontoPagado { get; private set; }
    public decimal? SaldoPendiente { get; private set; }
    public Guid? UsuarioAutorizoSaldoId { get; private set; }
    public string? ResultadoPruebas { get; private set; }

    public Diagnostico? Diagnostico { get; private set; }
    public CotizacionReparacion? CotizacionReparacion { get; private set; }

    private readonly List<ConsumoRepuesto> _consumosRepuesto = [];
    public IReadOnlyCollection<ConsumoRepuesto> ConsumosRepuesto => _consumosRepuesto.AsReadOnly();

    private OrdenTrabajo() { }

    public OrdenTrabajo(Guid clienteId, string equipoDescripcion, string fallaReportada, Guid usuarioRecepcionId, DateTime fechaRecepcion)
    {
        if (clienteId == Guid.Empty)
        {
            throw new ArgumentException("El cliente es obligatorio.", nameof(clienteId));
        }

        if (string.IsNullOrWhiteSpace(equipoDescripcion))
        {
            throw new ArgumentException("La descripción del equipo es obligatoria.", nameof(equipoDescripcion));
        }

        if (string.IsNullOrWhiteSpace(fallaReportada))
        {
            throw new ArgumentException("La falla reportada es obligatoria.", nameof(fallaReportada));
        }

        ClienteId = clienteId;
        EquipoDescripcion = equipoDescripcion;
        FallaReportada = fallaReportada;
        UsuarioRecepcionId = usuarioRecepcionId;
        FechaRecepcion = fechaRecepcion;
        Estado = EstadoOrdenTrabajo.Recibido;
    }

    /// <summary>UC-23/RF-054.</summary>
    public void RegistrarDiagnostico(string descripcion, Guid usuarioId, DateTime fecha)
    {
        if (Estado != EstadoOrdenTrabajo.Recibido)
        {
            throw new InvalidOperationException("Solo se puede registrar el diagnóstico cuando la OT está en estado Recibido.");
        }

        Diagnostico = new Diagnostico(Id, descripcion, usuarioId, fecha);
        Estado = EstadoOrdenTrabajo.Diagnosticado;
    }

    /// <summary>UC-24/RF-055.</summary>
    public void GenerarCotizacion(decimal montoEstimado, DateTime fecha)
    {
        if (Estado != EstadoOrdenTrabajo.Diagnosticado)
        {
            throw new InvalidOperationException("Solo se puede cotizar una OT en estado Diagnosticado.");
        }

        CotizacionReparacion = new CotizacionReparacion(Id, montoEstimado, fecha);
        Estado = EstadoOrdenTrabajo.Cotizado;
    }

    /// <summary>UC-24/RF-056 — RN-016: la reparación no puede iniciarse sin aprobación previa.</summary>
    public void RegistrarDecisionCliente(DecisionCliente decision, decimal? cobroDiagnosticoRechazo, string? evidenciaAprobacion)
    {
        if (Estado != EstadoOrdenTrabajo.Cotizado || CotizacionReparacion is null)
        {
            throw new InvalidOperationException("Solo se puede registrar la decisión del cliente sobre una OT cotizada.");
        }

        CotizacionReparacion.RegistrarDecision(decision, cobroDiagnosticoRechazo, evidenciaAprobacion);
        Estado = decision == DecisionCliente.Aprobada ? EstadoOrdenTrabajo.Aprobado : EstadoOrdenTrabajo.Rechazado;
    }

    /// <summary>UC-25/RF-057, RF-058 — RN-018: los repuestos se descuentan del inventario compartido
    /// (efecto aplicado por el handler de Application, no aquí, ya que requiere el repositorio de Producto).</summary>
    public void RegistrarReparacion(IEnumerable<(Guid ProductoId, decimal Cantidad)> consumos, string? resultadoPruebas)
    {
        if (Estado != EstadoOrdenTrabajo.Aprobado)
        {
            throw new InvalidOperationException("Solo se puede reparar una OT en estado Aprobado.");
        }

        foreach (var (productoId, cantidad) in consumos)
        {
            _consumosRepuesto.Add(new ConsumoRepuesto(Id, productoId, cantidad));
        }

        ResultadoPruebas = resultadoPruebas;
        Estado = EstadoOrdenTrabajo.ListoParaEntrega;
    }

    /// <summary>UC-26/RF-059 — RN-001: el pago no bloquea la entrega; RN-001/RN-031: el saldo
    /// pendiente exige el usuario Administrador/Propietario que autorizó.</summary>
    public void EntregarEquipo(
        EstadoPagoOrdenTrabajo estadoPago, decimal montoPagado, decimal? saldoPendiente,
        Guid? usuarioAutorizoSaldoId, Guid usuarioEntregaId, DateTime fechaEntrega)
    {
        if (Estado != EstadoOrdenTrabajo.ListoParaEntrega)
        {
            throw new InvalidOperationException("Solo se puede entregar una OT en estado ListoParaEntrega.");
        }

        if (montoPagado < 0)
        {
            throw new ArgumentException("El monto pagado no puede ser negativo.", nameof(montoPagado));
        }

        if (estadoPago == EstadoPagoOrdenTrabajo.SaldoPendiente)
        {
            if (saldoPendiente is not (> 0))
            {
                throw new ArgumentException("El saldo pendiente debe ser mayor a cero.", nameof(saldoPendiente));
            }

            if (usuarioAutorizoSaldoId is null)
            {
                throw new ArgumentException(
                    "Se requiere el usuario Administrador/Propietario que autorizó el saldo pendiente.", nameof(usuarioAutorizoSaldoId));
            }
        }

        EstadoPago = estadoPago;
        MontoPagado = montoPagado;
        SaldoPendiente = saldoPendiente;
        UsuarioAutorizoSaldoId = usuarioAutorizoSaldoId;
        UsuarioEntregaId = usuarioEntregaId;
        FechaEntrega = fechaEntrega;
        Estado = EstadoOrdenTrabajo.Entregado;
    }
}
