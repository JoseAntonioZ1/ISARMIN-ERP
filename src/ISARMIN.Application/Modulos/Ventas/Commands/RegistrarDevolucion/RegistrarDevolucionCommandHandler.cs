using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Ventas.DTOs;
using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Ventas.Commands.RegistrarDevolucion;

/// <summary>UC-17/RF-091 — RN-032: la devolución se registra como un movimiento de inventario
/// trazable a la venta original; no existe un módulo de gestión de devoluciones más complejo en V1.</summary>
public class RegistrarDevolucionCommandHandler : ICommandHandler<RegistrarDevolucionCommand, VentaDto>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RegistrarDevolucionCommand> _validator;

    public RegistrarDevolucionCommandHandler(
        IVentaRepository ventaRepository,
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoInventarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<RegistrarDevolucionCommand> validator)
    {
        _ventaRepository = ventaRepository;
        _productoRepository = productoRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<VentaDto> ManejarAsync(RegistrarDevolucionCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var venta = await _ventaRepository.ObtenerPorIdAsync(comando.VentaId, cancellationToken)
            ?? throw new VentaNoEncontradaException(comando.VentaId);

        var fecha = _fechaHoraProvider.UtcAhora;
        foreach (var detalle in comando.Detalles)
        {
            if (!venta.Detalles.Any(d => d.ProductoId == detalle.ProductoId))
            {
                throw new ProductoNoVendidoException(detalle.ProductoId, venta.Id);
            }

            var producto = await _productoRepository.ObtenerPorIdAsync(detalle.ProductoId, cancellationToken)
                ?? throw new ProductoNoEncontradoException(detalle.ProductoId);

            producto.AjustarStock(detalle.Cantidad);
            var movimiento = MovimientoInventario.CrearDevolucion(detalle.ProductoId, detalle.Cantidad, venta.Id, comando.UsuarioId, fecha);
            _movimientoInventarioRepository.Agregar(movimiento);
        }

        await _ventaRepository.GuardarCambiosAsync(cancellationToken);

        return venta.ADto();
    }
}
