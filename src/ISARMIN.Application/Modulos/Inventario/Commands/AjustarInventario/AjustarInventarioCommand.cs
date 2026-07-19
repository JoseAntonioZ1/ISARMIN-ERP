namespace ISARMIN.Application.Modulos.Inventario.Commands.AjustarInventario;

public record AjustarInventarioCommand(Guid ProductoId, decimal CantidadAjuste, string Motivo, Guid UsuarioId);
