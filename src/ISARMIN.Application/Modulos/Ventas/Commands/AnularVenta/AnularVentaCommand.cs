namespace ISARMIN.Application.Modulos.Ventas.Commands.AnularVenta;

public record AnularVentaCommand(Guid VentaId, string Motivo, Guid UsuarioId);
