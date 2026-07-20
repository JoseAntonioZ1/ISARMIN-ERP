using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.ServiciosCampo;

/// <summary>UC-30 a UC-33 — Servicio de Campo (RF-064 a RF-070). Más simple que Taller: no tiene una
/// máquina de estados con aprobación formal de cotización — <see cref="Cerrar"/> transiciona directo
/// de Solicitado a Cerrado, combinando consumo de materiales y conformidad (RN-019) en un solo paso.</summary>
public class ServicioCampo : Entity
{
    public Guid ClienteId { get; private set; }
    public string DescripcionTrabajo { get; private set; } = null!;
    public DateTime FechaSolicitud { get; private set; }
    public Guid? TecnicoAsignadoId { get; private set; }
    public EstadoServicioCampo Estado { get; private set; }

    public decimal? MontoEstimado { get; private set; }

    public DateTime? FechaEjecucion { get; private set; }
    public string? EstadoFinal { get; private set; }
    public string? Observaciones { get; private set; }
    public Guid? UsuarioCierreId { get; private set; }

    public Guid? MedioPagoId { get; private set; }
    public decimal? MontoPagado { get; private set; }
    public decimal? SaldoPendiente { get; private set; }
    public Guid? UsuarioAutorizoSaldoId { get; private set; }

    private readonly List<ServicioCampoDetalle> _detalles = [];
    public IReadOnlyCollection<ServicioCampoDetalle> Detalles => _detalles.AsReadOnly();

    private ServicioCampo() { }

    public ServicioCampo(Guid clienteId, string descripcionTrabajo, Guid? tecnicoAsignadoId, DateTime fechaSolicitud)
    {
        if (clienteId == Guid.Empty)
        {
            throw new ArgumentException("El cliente es obligatorio.", nameof(clienteId));
        }

        if (string.IsNullOrWhiteSpace(descripcionTrabajo))
        {
            throw new ArgumentException("La descripción del trabajo es obligatoria.", nameof(descripcionTrabajo));
        }

        ClienteId = clienteId;
        DescripcionTrabajo = descripcionTrabajo;
        TecnicoAsignadoId = tecnicoAsignadoId;
        FechaSolicitud = fechaSolicitud;
        Estado = EstadoServicioCampo.Solicitado;
    }

    /// <summary>UC-31/RF-068 — análogo a la cotización de Taller, pero solo registra el monto
    /// estimado (el catálogo de estados confirmado no tiene un paso "Cotizado" con aprobación formal).</summary>
    public void Cotizar(decimal montoEstimado)
    {
        if (Estado != EstadoServicioCampo.Solicitado)
        {
            throw new InvalidOperationException("Solo se puede cotizar un servicio en estado Solicitado.");
        }

        if (montoEstimado < 0)
        {
            throw new ArgumentException("El monto estimado no puede ser negativo.", nameof(montoEstimado));
        }

        MontoEstimado = montoEstimado;
    }

    /// <summary>UC-32/RF-067, RF-069 — RN-019: cierre con estado final, observaciones y responsable.</summary>
    public void Cerrar(IEnumerable<(Guid ProductoId, decimal Cantidad)> consumos, string estadoFinal, string? observaciones, Guid usuarioCierreId, DateTime fechaEjecucion)
    {
        if (Estado != EstadoServicioCampo.Solicitado)
        {
            throw new InvalidOperationException("Solo se puede cerrar un servicio en estado Solicitado.");
        }

        if (string.IsNullOrWhiteSpace(estadoFinal))
        {
            throw new ArgumentException("El estado final del servicio es obligatorio.", nameof(estadoFinal));
        }

        foreach (var (productoId, cantidad) in consumos)
        {
            _detalles.Add(new ServicioCampoDetalle(Id, productoId, cantidad));
        }

        EstadoFinal = estadoFinal;
        Observaciones = observaciones;
        UsuarioCierreId = usuarioCierreId;
        FechaEjecucion = fechaEjecucion;
        Estado = EstadoServicioCampo.Cerrado;
    }

    /// <summary>UC-33/RF-070 — RN-031: no bloquea el cierre ya ocurrido; el saldo pendiente exige el
    /// usuario Administrador/Propietario autorizante.</summary>
    public void RegistrarCobro(Guid medioPagoId, decimal montoPagado, decimal? saldoPendiente, Guid? usuarioAutorizoSaldoId)
    {
        if (Estado != EstadoServicioCampo.Cerrado)
        {
            throw new InvalidOperationException("Solo se puede registrar el cobro de un servicio cerrado.");
        }

        if (montoPagado < 0)
        {
            throw new ArgumentException("El monto pagado no puede ser negativo.", nameof(montoPagado));
        }

        if (saldoPendiente is { } saldo)
        {
            if (saldo <= 0)
            {
                throw new ArgumentException("El saldo pendiente debe ser mayor a cero.", nameof(saldoPendiente));
            }

            if (usuarioAutorizoSaldoId is null)
            {
                throw new ArgumentException(
                    "Se requiere el usuario Administrador/Propietario que autorizó el saldo pendiente.", nameof(usuarioAutorizoSaldoId));
            }
        }

        MedioPagoId = medioPagoId;
        MontoPagado = montoPagado;
        SaldoPendiente = saldoPendiente;
        UsuarioAutorizoSaldoId = usuarioAutorizoSaldoId;
    }
}
