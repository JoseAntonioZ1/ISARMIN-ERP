namespace ISARMIN.Application.Modulos.Inventario.Commands.EditarProducto;

public record EditarProductoCommand(
    Guid Id,
    string CodigoInterno,
    string Nombre,
    Guid CategoriaId,
    Guid UnidadMedidaId,
    decimal CostoReferencia,
    decimal PrecioVenta,
    string? Marca,
    string? CodigoBarras,
    decimal? StockMinimo);
