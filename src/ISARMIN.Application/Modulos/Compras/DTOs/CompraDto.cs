using ISARMIN.Domain.Entities.Compras;

namespace ISARMIN.Application.Modulos.Compras.DTOs;

public record CompraDetalleDto(Guid Id, Guid ProductoId, decimal Cantidad, decimal CostoUnitario);

public record CompraDto(
    Guid Id,
    Guid ProveedorId,
    DateOnly Fecha,
    string DocumentoCompraTipo,
    string DocumentoCompraNumero,
    Guid UsuarioId,
    decimal Total,
    IReadOnlyCollection<CompraDetalleDto> Detalles);

public static class CompraMapper
{
    public static CompraDto ADto(this Compra compra) => new(
        compra.Id,
        compra.ProveedorId,
        compra.Fecha,
        compra.DocumentoCompraTipo,
        compra.DocumentoCompraNumero,
        compra.UsuarioId,
        compra.Total,
        compra.Detalles.Select(d => new CompraDetalleDto(d.Id, d.ProductoId, d.Cantidad, d.CostoUnitario)).ToList());
}
