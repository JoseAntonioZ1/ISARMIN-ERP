namespace ISARMIN.Application.Modulos.Ventas.Commands.RegistrarDevolucion;

public record DetalleDevolucionInput(Guid ProductoId, decimal Cantidad);

public record RegistrarDevolucionCommand(
    Guid VentaId,
    IReadOnlyCollection<DetalleDevolucionInput> Detalles,
    string? Motivo,
    Guid UsuarioId);
