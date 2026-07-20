namespace ISARMIN.Domain.Enums;

/// <summary>Solo Solicitado y Cerrado son alcanzables por los comandos actuales: Agendado/EnEjecucion
/// se declaran para completar el catálogo confirmado, pero <c>CerrarServicioCampoCommand</c> transiciona
/// directo de Solicitado a Cerrado (mismo patrón que Taller: API-Design.md define un único endpoint
/// de cierre, sin pasos intermedios expuestos).</summary>
public enum EstadoServicioCampo
{
    Solicitado,
    Agendado,
    EnEjecucion,
    Cerrado
}
