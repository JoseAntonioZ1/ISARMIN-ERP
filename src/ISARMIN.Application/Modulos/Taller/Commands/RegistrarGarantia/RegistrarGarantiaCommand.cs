namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarGarantia;

public record RegistrarGarantiaCommand(Guid OrdenTrabajoId, DateOnly FechaInicio, DateOnly FechaFin);
