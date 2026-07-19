namespace ISARMIN.Application.Modulos.Usuarios.Commands.RestablecerCredencial;

/// <summary>RN-038 — restablecimiento de contraseña exclusivo del Administrador.</summary>
public record RestablecerCredencialCommand(Guid Id, string NuevaCredencial);
