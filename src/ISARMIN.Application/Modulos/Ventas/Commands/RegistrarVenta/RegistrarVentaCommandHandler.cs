using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Application.Modulos.Ventas.DTOs;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Entities.Ventas;

namespace ISARMIN.Application.Modulos.Ventas.Commands.RegistrarVenta;

/// <summary>UC-14/RF-038 a RF-041, RF-089 — RN-003: valida stock antes de confirmar; RN-031: el
/// saldo pendiente exige un usuario Administrador/Propietario autorizante.</summary>
public class RegistrarVentaCommandHandler : ICommandHandler<RegistrarVentaCommand, VentaDto>
{
    private readonly IVentaRepository _ventaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IMedioPagoRepository _medioPagoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RegistrarVentaCommand> _validator;

    public RegistrarVentaCommandHandler(
        IVentaRepository ventaRepository,
        IClienteRepository clienteRepository,
        IProductoRepository productoRepository,
        IMedioPagoRepository medioPagoRepository,
        IUsuarioRepository usuarioRepository,
        IMovimientoInventarioRepository movimientoInventarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<RegistrarVentaCommand> validator)
    {
        _ventaRepository = ventaRepository;
        _clienteRepository = clienteRepository;
        _productoRepository = productoRepository;
        _medioPagoRepository = medioPagoRepository;
        _usuarioRepository = usuarioRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<VentaDto> ManejarAsync(RegistrarVentaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        if (comando.ClienteId is { } clienteId && await _clienteRepository.ObtenerPorIdAsync(clienteId, cancellationToken) is null)
        {
            throw new ClienteNoEncontradoException(clienteId);
        }

        var productos = new Dictionary<Guid, Producto>();
        foreach (var detalle in comando.Detalles)
        {
            if (productos.ContainsKey(detalle.ProductoId))
            {
                continue;
            }

            var producto = await _productoRepository.ObtenerPorIdAsync(detalle.ProductoId, cancellationToken)
                ?? throw new ProductoNoEncontradoException(detalle.ProductoId);
            productos[detalle.ProductoId] = producto;
        }

        var cantidadPorProducto = comando.Detalles
            .GroupBy(d => d.ProductoId)
            .ToDictionary(g => g.Key, g => g.Sum(d => d.Cantidad));

        foreach (var (productoId, cantidadTotal) in cantidadPorProducto)
        {
            if (productos[productoId].StockActual < cantidadTotal)
            {
                throw new StockInsuficienteException(productoId);
            }
        }

        foreach (var pago in comando.Pagos)
        {
            if (await _medioPagoRepository.ObtenerPorIdAsync(pago.MedioPagoId, cancellationToken) is null)
            {
                throw new MedioPagoNoEncontradoException(pago.MedioPagoId);
            }
        }

        if (comando.UsuarioAutorizoSaldoId is { } usuarioAutorizoId
            && await _usuarioRepository.ObtenerPorIdAsync(usuarioAutorizoId, cancellationToken) is null)
        {
            throw new UsuarioNoEncontradoException(usuarioAutorizoId);
        }

        var fecha = _fechaHoraProvider.UtcAhora;
        var venta = new Venta(
            comando.ClienteId,
            comando.TipoComprobante,
            comando.UsuarioId,
            fecha,
            comando.Detalles.Select(d => (d.ProductoId, d.Cantidad, d.PrecioUnitario)),
            comando.Pagos.Select(p => (p.MedioPagoId, p.Monto)),
            comando.UsuarioAutorizoSaldoId);

        foreach (var (productoId, cantidadTotal) in cantidadPorProducto)
        {
            productos[productoId].AjustarStock(-cantidadTotal);
            var movimiento = MovimientoInventario.CrearVenta(productoId, cantidadTotal, venta.Id, comando.UsuarioId, fecha);
            _movimientoInventarioRepository.Agregar(movimiento);
        }

        _ventaRepository.Agregar(venta);
        await _ventaRepository.GuardarCambiosAsync(cancellationToken);

        return venta.ADto();
    }
}
