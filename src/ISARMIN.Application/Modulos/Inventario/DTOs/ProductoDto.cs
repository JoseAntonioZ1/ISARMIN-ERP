using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.DTOs;

public record ProductoDto(
    Guid Id,
    string CodigoInterno,
    string? CodigoBarras,
    string Nombre,
    Guid CategoriaId,
    string? Marca,
    Guid UnidadMedidaId,
    decimal CostoReferencia,
    decimal PrecioVenta,
    decimal Margen,
    decimal StockActual,
    decimal? StockMinimo,
    string Estado);

public static class ProductoMapper
{
    public static ProductoDto ADto(this Producto producto) => new(
        producto.Id,
        producto.CodigoInterno,
        producto.CodigoBarras,
        producto.Nombre,
        producto.CategoriaId,
        producto.Marca,
        producto.UnidadMedidaId,
        producto.CostoReferencia,
        producto.PrecioVenta,
        producto.Margen,
        producto.StockActual,
        producto.StockMinimo,
        producto.Estado.ToString());
}
