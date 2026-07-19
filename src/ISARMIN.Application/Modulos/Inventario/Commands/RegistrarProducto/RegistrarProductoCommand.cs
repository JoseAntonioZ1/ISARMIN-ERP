namespace ISARMIN.Application.Modulos.Inventario.Commands.RegistrarProducto;

public record RegistrarProductoCommand(
    string CodigoInterno,
    string Nombre,
    Guid CategoriaId,
    Guid UnidadMedidaId,
    decimal CostoReferencia,
    decimal PrecioVenta,
    decimal StockInicial,
    string? Marca,
    string? CodigoBarras,
    decimal? StockMinimo);
