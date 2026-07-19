using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios.DTOs;

public record PermisoDto(string Modulo, string Accion);

public record RolDto(Guid Id, string Nombre, string? Descripcion, IReadOnlyCollection<PermisoDto> Permisos);

public static class RolMapper
{
    public static RolDto ADto(this Rol rol) => new(
        rol.Id,
        rol.Nombre,
        rol.Descripcion,
        rol.Permisos.Select(p => new PermisoDto(p.Modulo, p.Accion.ToString())).ToList());
}
