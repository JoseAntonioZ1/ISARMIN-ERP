namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarReparacion;

public record DetalleConsumoInput(Guid ProductoId, decimal Cantidad);

public record RegistrarReparacionCommand(
    Guid OrdenTrabajoId, IReadOnlyCollection<DetalleConsumoInput> Consumos, string? ResultadoPruebas, Guid UsuarioId);
