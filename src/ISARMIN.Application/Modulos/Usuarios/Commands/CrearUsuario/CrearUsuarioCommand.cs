namespace ISARMIN.Application.Modulos.Usuarios.Commands.CrearUsuario;

public record CrearUsuarioCommand(
    string Nombre,
    string NombreUsuario,
    string CredencialInicial,
    IReadOnlyCollection<Guid> RolIds);
