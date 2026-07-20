namespace ISARMIN.Domain.Enums;

/// <summary>Catálogo confirmado en el modelo físico. Emitida no se persiste como paso real en V1
/// (no hay integración SUNAT, RF-044/UC-15 diferido): una venta pasa directo de Registrada a Pagada
/// si no queda saldo pendiente, o permanece Registrada si lo hay.</summary>
public enum EstadoVenta
{
    Registrada,
    Emitida,
    Pagada,
    Anulada
}
