using ISARMIN.Domain.Common;

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
}
