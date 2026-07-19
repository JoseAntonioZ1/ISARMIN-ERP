namespace ISARMIN.Domain.Entities.Identidad;

/// <summary>Relación N:M entre Usuario y Rol (clave compuesta, sin Id propio — BQ-042).</summary>
public class UsuarioRol
{
    public Guid UsuarioId { get; private set; }
    public Guid RolId { get; private set; }

    public Rol Rol { get; private set; } = null!;

    private UsuarioRol() { }

    public UsuarioRol(Guid usuarioId, Guid rolId)
    {
        UsuarioId = usuarioId;
        RolId = rolId;
    }
}
