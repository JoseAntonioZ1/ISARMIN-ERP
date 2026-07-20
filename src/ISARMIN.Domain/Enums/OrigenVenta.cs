namespace ISARMIN.Domain.Enums;

/// <summary>RF-083 — vincular una venta a una OT o Servicio de Campo (para combinar materiales y
/// mano de obra en un solo comprobante) queda fuera de alcance en esta implementación; el catálogo
/// se declara completo para coincidir con el CHECK ya confirmado, pero solo <see cref="Directa"/>
/// es alcanzable por los comandos actuales.</summary>
public enum OrigenVenta
{
    Directa,
    OrdenTrabajo,
    ServicioCampo
}
