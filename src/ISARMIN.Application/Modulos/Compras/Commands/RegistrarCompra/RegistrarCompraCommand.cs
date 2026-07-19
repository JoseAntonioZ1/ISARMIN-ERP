namespace ISARMIN.Application.Modulos.Compras.Commands.RegistrarCompra;

public record DetalleCompraInput(Guid ProductoId, decimal Cantidad, decimal CostoUnitario);

public record RegistrarCompraCommand(
    Guid ProveedorId,
    DateOnly Fecha,
    string DocumentoCompraTipo,
    string DocumentoCompraNumero,
    Guid UsuarioId,
    IReadOnlyCollection<DetalleCompraInput> Detalles);
