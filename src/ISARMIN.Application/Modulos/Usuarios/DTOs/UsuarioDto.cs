namespace ISARMIN.Application.Modulos.Usuarios.DTOs;

public record RolResumenDto(Guid Id, string Nombre);

public record UsuarioDto(
    Guid Id,
    string Nombre,
    string NombreUsuario,
    string Estado,
    IReadOnlyCollection<RolResumenDto> Roles);
