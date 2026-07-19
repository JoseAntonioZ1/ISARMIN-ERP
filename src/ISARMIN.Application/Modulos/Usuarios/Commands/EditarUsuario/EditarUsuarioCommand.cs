namespace ISARMIN.Application.Modulos.Usuarios.Commands.EditarUsuario;

public record EditarUsuarioCommand(Guid Id, string Nombre, IReadOnlyCollection<Guid> RolIds);
