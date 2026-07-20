namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarRecepcion;

public record RegistrarRecepcionCommand(Guid ClienteId, string EquipoDescripcion, string FallaReportada, Guid UsuarioId);
