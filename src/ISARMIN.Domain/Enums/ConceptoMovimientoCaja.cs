namespace ISARMIN.Domain.Enums;

/// <summary>RN-026 — concepto explícito de un movimiento de caja manual (sin origen transaccional),
/// para separar el dinero del negocio del dinero personal del propietario.</summary>
public enum ConceptoMovimientoCaja
{
    GastoOperativo,
    RetiroPropietario,
    AporteCapital
}
