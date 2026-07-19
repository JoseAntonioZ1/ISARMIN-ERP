namespace ISARMIN.Application.Modulos.Usuarios.Commands.CambiarEstadoUsuario;

/// <summary>RF-003/RN-021 — baja lógica (Activo = false desactiva, nunca elimina físicamente).</summary>
public record CambiarEstadoUsuarioCommand(Guid Id, bool Activo);
