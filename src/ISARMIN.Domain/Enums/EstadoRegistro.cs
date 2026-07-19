namespace ISARMIN.Domain.Enums;

/// <summary>
/// Baja lógica compartida por Usuario, Cliente, Proveedor y Producto (RN-023):
/// nunca se elimina físicamente un registro para preservar su historial.
/// </summary>
public enum EstadoRegistro
{
    Activo,
    Inactivo
}
