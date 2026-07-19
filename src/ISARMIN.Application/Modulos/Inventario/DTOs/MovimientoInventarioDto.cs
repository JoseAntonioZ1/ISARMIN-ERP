using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.DTOs;

public record MovimientoInventarioDto(
    Guid Id,
    Guid ProductoId,
    string TipoMovimiento,
    decimal Cantidad,
    string? OrigenTipo,
    Guid? OrigenId,
    string? MotivoAjuste,
    Guid UsuarioId,
    DateTime Fecha);

public static class MovimientoInventarioMapper
{
    public static MovimientoInventarioDto ADto(this MovimientoInventario movimiento) => new(
        movimiento.Id,
        movimiento.ProductoId,
        movimiento.TipoMovimiento.ToString(),
        movimiento.Cantidad,
        movimiento.OrigenTipo,
        movimiento.OrigenId,
        movimiento.MotivoAjuste,
        movimiento.UsuarioId,
        movimiento.Fecha);
}
