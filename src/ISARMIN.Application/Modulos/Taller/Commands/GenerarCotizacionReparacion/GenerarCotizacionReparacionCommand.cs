namespace ISARMIN.Application.Modulos.Taller.Commands.GenerarCotizacionReparacion;

public record GenerarCotizacionReparacionCommand(Guid OrdenTrabajoId, decimal MontoEstimado);
