namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarDiagnostico;

public record RegistrarDiagnosticoCommand(Guid OrdenTrabajoId, string Descripcion, Guid UsuarioId);
