using ISARMIN.Domain.Entities.Ventas;

namespace ISARMIN.Application.Modulos.Ventas.DTOs;

public record VentaDetalleDto(Guid Id, Guid ProductoId, decimal Cantidad, decimal PrecioUnitario);

public record PagoVentaDto(Guid Id, Guid MedioPagoId, decimal Monto);

public record VentaDto(
    Guid Id,
    Guid? ClienteId,
    string TipoComprobante,
    string Origen,
    DateTime Fecha,
    Guid UsuarioId,
    decimal Total,
    string Estado,
    decimal? SaldoPendiente,
    Guid? UsuarioAutorizoSaldoId,
    string? MotivoAnulacion,
    Guid? UsuarioAnuloId,
    DateTime? FechaAnulacion,
    IReadOnlyCollection<VentaDetalleDto> Detalles,
    IReadOnlyCollection<PagoVentaDto> Pagos);

public static class VentaMapper
{
    public static VentaDto ADto(this Venta venta) => new(
        venta.Id,
        venta.ClienteId,
        venta.TipoComprobante.ToString(),
        venta.Origen.ToString(),
        venta.Fecha,
        venta.UsuarioId,
        venta.Total,
        venta.Estado.ToString(),
        venta.SaldoPendiente,
        venta.UsuarioAutorizoSaldoId,
        venta.MotivoAnulacion,
        venta.UsuarioAnuloId,
        venta.FechaAnulacion,
        venta.Detalles.Select(d => new VentaDetalleDto(d.Id, d.ProductoId, d.Cantidad, d.PrecioUnitario)).ToList(),
        venta.Pagos.Select(p => new PagoVentaDto(p.Id, p.MedioPagoId, p.Monto)).ToList());
}
