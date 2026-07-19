namespace ISARMIN.Application.Modulos.Usuarios.DTOs;

public record UsuarioSesionDto(Guid Id, string Nombre);

public record SesionDto(
    string Token,
    DateTime ExpiraEn,
    UsuarioSesionDto Usuario,
    IReadOnlyCollection<string> Permisos);
