namespace ISARMIN.Domain.Enums;

/// <summary>Máquina de estados de la OT (Business-States.md). Solo Recibido, Diagnosticado, Cotizado,
/// Aprobado, Rechazado, ListoParaEntrega y Entregado son alcanzables por los comandos actuales:
/// EnReparacion/EnPruebas se declaran para completar el catálogo confirmado, pero <c>RegistrarReparacionCommand</c>
/// combina ambos pasos y transiciona directo de Aprobado a ListoParaEntrega (API-Design.md define un único endpoint).</summary>
public enum EstadoOrdenTrabajo
{
    Recibido,
    Diagnosticado,
    Cotizado,
    Aprobado,
    Rechazado,
    EnReparacion,
    EnPruebas,
    ListoParaEntrega,
    Entregado
}
