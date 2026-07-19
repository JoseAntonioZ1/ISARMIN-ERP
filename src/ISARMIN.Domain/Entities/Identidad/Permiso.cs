using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Identidad;

public class Permiso : Entity
{
    public Guid RolId { get; private set; }
    public string Modulo { get; private set; } = null!;
    public AccionPermiso Accion { get; private set; }

    private Permiso() { }

    public Permiso(Guid rolId, string modulo, AccionPermiso accion)
    {
        if (string.IsNullOrWhiteSpace(modulo))
        {
            throw new ArgumentException("El módulo del permiso es obligatorio.", nameof(modulo));
        }

        RolId = rolId;
        Modulo = modulo;
        Accion = accion;
    }

    /// <summary>Nombre lógico usado en las políticas de autorización, ej. "Inventario.Ajustar".</summary>
    public string NombreLogico => $"{Modulo}.{Accion}";
}
