namespace ISARMIN.Application.Modulos.Usuarios.Commands.AsignarPermisos;

/// <summary>Accion es texto (no el enum) porque cruza el límite de la API — se valida contra AccionPermiso en el Validator.</summary>
public record PermisoAsignacion(string Modulo, string Accion);

public record AsignarPermisosCommand(Guid RolId, IReadOnlyCollection<PermisoAsignacion> Permisos);
