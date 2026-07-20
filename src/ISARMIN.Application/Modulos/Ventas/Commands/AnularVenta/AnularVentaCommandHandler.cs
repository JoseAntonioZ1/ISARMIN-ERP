using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Ventas.DTOs;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Ventas.Commands.AnularVenta;

/// <summary>UC-16/RF-043 — RN-010: motivo obligatorio, reversión automática de inventario. La ventana
/// de tiempo exacta para anular no está confirmada (BQ-013); se resuelve con el permiso
/// `Ventas.Anular` como único control de acceso, sin inventar una restricción de horas.</summary>
public class AnularVentaCommandHandler : ICommandHandler<AnularVentaCommand, VentaDto>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<AnularVentaCommand> _validator;

    public AnularVentaCommandHandler(
        IVentaRepository ventaRepository,
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoInventarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<AnularVentaCommand> validator)
    {
        _ventaRepository = ventaRepository;
        _productoRepository = productoRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<VentaDto> ManejarAsync(AnularVentaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var venta = await _ventaRepository.ObtenerPorIdAsync(comando.VentaId, cancellationToken)
            ?? throw new VentaNoEncontradaException(comando.VentaId);

        if (venta.Estado == EstadoVenta.Anulada)
        {
            throw new EstadoVentaInvalidoException(venta.Id, "La venta ya se encuentra anulada.");
        }

        var fecha = _fechaHoraProvider.UtcAhora;
        foreach (var detalle in venta.Detalles)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(detalle.ProductoId, cancellationToken)
                ?? throw new ProductoNoEncontradoException(detalle.ProductoId);

            producto.AjustarStock(detalle.Cantidad);
            var movimiento = MovimientoInventario.CrearDevolucion(detalle.ProductoId, detalle.Cantidad, venta.Id, comando.UsuarioId, fecha);
            _movimientoInventarioRepository.Agregar(movimiento);
        }

        venta.Anular(comando.Motivo, comando.UsuarioId, fecha);

        await _ventaRepository.GuardarCambiosAsync(cancellationToken);

        return venta.ADto();
    }
}
