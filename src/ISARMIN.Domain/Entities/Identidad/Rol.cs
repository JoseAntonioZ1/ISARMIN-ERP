using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Identidad;

public class Rol : Entity
{
    public string Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }

    private readonly List<Permiso> _permisos = [];
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly();

    private Rol() { }

    public Rol(string nombre, string? descripcion = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del rol es obligatorio.", nameof(nombre));
        }

        Nombre = nombre;
        Descripcion = descripcion;
    }

    /// <summary>RF-008/UC-04, paso 1 — crear o editar un rol (nombre, descripción).</summary>
    public void ActualizarDatos(string nombre, string? descripcion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre del rol es obligatorio.", nameof(nombre));
        }

        Nombre = nombre;
        Descripcion = descripcion;
    }

    public void AsignarPermiso(string modulo, AccionPermiso accion)
    {
        if (_permisos.Any(p => p.Modulo == modulo && p.Accion == accion))
        {
            return;
        }

        _permisos.Add(new Permiso(Id, modulo, accion));
    }

    public void QuitarPermiso(string modulo, AccionPermiso accion) =>
        _permisos.RemoveAll(p => p.Modulo == modulo && p.Accion == accion);

    /// <summary>RF-010/UC-04, paso 2 — reemplaza el conjunto completo de permisos asignados al rol.</summary>
    public void ReemplazarPermisos(IEnumerable<(string Modulo, AccionPermiso Accion)> permisos)
    {
        var deseados = permisos.ToHashSet();

        foreach (var actual in _permisos.Select(p => (p.Modulo, p.Accion)).ToList().Where(actual => !deseados.Contains(actual)))
        {
            QuitarPermiso(actual.Modulo, actual.Accion);
        }

        foreach (var (modulo, accion) in deseados)
        {
            AsignarPermiso(modulo, accion);
        }
    }
}
