namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CerrarServicioCampo;

public record DetalleConsumoCampoInput(Guid ProductoId, decimal Cantidad);

public record CerrarServicioCampoCommand(
    Guid ServicioCampoId,
    IReadOnlyCollection<DetalleConsumoCampoInput> Consumos,
    string EstadoFinal,
    string? Observaciones,
    Guid UsuarioId);
