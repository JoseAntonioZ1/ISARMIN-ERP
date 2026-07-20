using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Taller;

/// <summary>UC-24/RF-055, RF-056 — cotización de reparación, 1:1 con su Orden de Trabajo.</summary>
public class CotizacionReparacion : Entity
{
    public Guid OrdenTrabajoId { get; private set; }
    public decimal MontoEstimado { get; private set; }
    public DateTime Fecha { get; private set; }
    public DecisionCliente? DecisionCliente { get; private set; }

    /// <summary>RN-030 — solo aplica si el cliente rechazó la cotización; no se asume gratuito ni costo fijo.</summary>
    public decimal? CobroDiagnosticoRechazo { get; private set; }

    /// <summary>Forma exacta sin confirmar (BQ-034) — texto libre por ahora.</summary>
    public string? EvidenciaAprobacion { get; private set; }

    private CotizacionReparacion() { }

    public CotizacionReparacion(Guid ordenTrabajoId, decimal montoEstimado, DateTime fecha)
    {
        if (montoEstimado < 0)
        {
            throw new ArgumentException("El monto estimado no puede ser negativo.", nameof(montoEstimado));
        }

        OrdenTrabajoId = ordenTrabajoId;
        MontoEstimado = montoEstimado;
        Fecha = fecha;
    }

    /// <summary>RN-016/RN-030 — registra la decisión del cliente una sola vez.</summary>
    public void RegistrarDecision(DecisionCliente decision, decimal? cobroDiagnosticoRechazo, string? evidenciaAprobacion)
    {
        if (DecisionCliente is not null)
        {
            throw new InvalidOperationException("La decisión del cliente ya fue registrada para esta cotización.");
        }

        if (decision == Enums.DecisionCliente.Aprobada && cobroDiagnosticoRechazo is not null)
        {
            throw new ArgumentException(
                "El cobro por diagnóstico solo aplica cuando el cliente rechaza la cotización.", nameof(cobroDiagnosticoRechazo));
        }

        if (cobroDiagnosticoRechazo is < 0)
        {
            throw new ArgumentException("El cobro por diagnóstico no puede ser negativo.", nameof(cobroDiagnosticoRechazo));
        }

        DecisionCliente = decision;
        CobroDiagnosticoRechazo = cobroDiagnosticoRechazo;
        EvidenciaAprobacion = evidenciaAprobacion;
    }
}
