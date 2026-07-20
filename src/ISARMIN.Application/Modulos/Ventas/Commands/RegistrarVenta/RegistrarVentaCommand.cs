using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Ventas.Commands.RegistrarVenta;

public record DetalleVentaInput(Guid ProductoId, decimal Cantidad, decimal PrecioUnitario);

public record PagoVentaInput(Guid MedioPagoId, decimal Monto);

public record RegistrarVentaCommand(
    Guid? ClienteId,
    TipoComprobante TipoComprobante,
    IReadOnlyCollection<DetalleVentaInput> Detalles,
    IReadOnlyCollection<PagoVentaInput> Pagos,
    Guid? UsuarioAutorizoSaldoId,
    Guid UsuarioId);
