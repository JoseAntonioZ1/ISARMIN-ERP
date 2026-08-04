namespace ISARMIN.Application.Modulos.Usuarios.DTOs;

public record UsuarioSesionDto(Guid Id, string Nombre);

public record SesionDto(
    string Token,
    DateTime ExpiraEn,
    string RefreshToken,
    DateTime RefreshTokenExpiraEn,
    UsuarioSesionDto Usuario,
    IReadOnlyCollection<string> Permisos);
