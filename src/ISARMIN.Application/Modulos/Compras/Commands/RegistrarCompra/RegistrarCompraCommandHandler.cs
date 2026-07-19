using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Compras.DTOs;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Proveedores;
using ISARMIN.Domain.Entities.Compras;
using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Compras.Commands.RegistrarCompra;

/// <summary>UC-13 — Registrar Compra (RF-033 a RF-035): compra directa ya realizada (RN-024), sin
/// orden de compra ni aprobación previa. Actualiza automáticamente stock, Kardex y costo de
/// referencia por costo promedio ponderado (RN-013).</summary>
public class RegistrarCompraCommandHandler : ICommandHandler<RegistrarCompraCommand, CompraDto>
{
    private readonly ICompraRepository _compraRepository;
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RegistrarCompraCommand> _validator;

    public RegistrarCompraCommandHandler(
        ICompraRepository compraRepository,
        IProveedorRepository proveedorRepository,
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoInventarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<RegistrarCompraCommand> validator)
    {
        _compraRepository = compraRepository;
        _proveedorRepository = proveedorRepository;
        _productoRepository = productoRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<CompraDto> ManejarAsync(RegistrarCompraCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        if (await _proveedorRepository.ObtenerPorIdAsync(comando.ProveedorId, cancellationToken) is null)
        {
            throw new ProveedorNoEncontradoException(comando.ProveedorId);
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

        var compra = new Compra(
            comando.ProveedorId,
            comando.Fecha,
            comando.DocumentoCompraTipo,
            comando.DocumentoCompraNumero,
            comando.UsuarioId,
            comando.Detalles.Select(d => (d.ProductoId, d.Cantidad, d.CostoUnitario)));

        var fechaMovimiento = _fechaHoraProvider.UtcAhora;
        foreach (var detalle in comando.Detalles)
        {
            var producto = productos[detalle.ProductoId];
            producto.RegistrarCompra(detalle.Cantidad, detalle.CostoUnitario);

            var movimiento = MovimientoInventario.CrearCompra(detalle.ProductoId, detalle.Cantidad, compra.Id, comando.UsuarioId, fechaMovimiento);
            _movimientoInventarioRepository.Agregar(movimiento);
        }

        _compraRepository.Agregar(compra);
        await _compraRepository.GuardarCambiosAsync(cancellationToken);

        return compra.ADto();
    }
}
