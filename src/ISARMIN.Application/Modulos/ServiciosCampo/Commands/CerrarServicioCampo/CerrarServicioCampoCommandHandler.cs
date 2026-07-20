using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CerrarServicioCampo;

/// <summary>UC-32/RF-067, RF-069 — RN-020: los materiales consumidos se descuentan del inventario
/// compartido. El servicio debe estar en estado Solicitado.</summary>
public class CerrarServicioCampoCommandHandler : ICommandHandler<CerrarServicioCampoCommand, ServicioCampoDto>
{
    private readonly IServicioCampoRepository _servicioCampoRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<CerrarServicioCampoCommand> _validator;

    public CerrarServicioCampoCommandHandler(
        IServicioCampoRepository servicioCampoRepository,
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoInventarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<CerrarServicioCampoCommand> validator)
    {
        _servicioCampoRepository = servicioCampoRepository;
        _productoRepository = productoRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<ServicioCampoDto> ManejarAsync(CerrarServicioCampoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var servicio = await _servicioCampoRepository.ObtenerPorIdAsync(comando.ServicioCampoId, cancellationToken)
            ?? throw new ServicioCampoNoEncontradoException(comando.ServicioCampoId);

        if (servicio.Estado != EstadoServicioCampo.Solicitado)
        {
            throw new EstadoServicioCampoInvalidoException(servicio.Id, "Solo se puede cerrar un servicio en estado Solicitado.");
        }

        var productos = new Dictionary<Guid, Producto>();
        foreach (var detalle in comando.Consumos)
        {
            if (productos.ContainsKey(detalle.ProductoId))
            {
                continue;
            }

            var producto = await _productoRepository.ObtenerPorIdAsync(detalle.ProductoId, cancellationToken)
                ?? throw new ProductoNoEncontradoException(detalle.ProductoId);
            productos[detalle.ProductoId] = producto;
        }

        var fechaMovimiento = _fechaHoraProvider.UtcAhora;
        foreach (var detalle in comando.Consumos)
        {
            var producto = productos[detalle.ProductoId];
            if (producto.StockActual < detalle.Cantidad)
            {
                throw new StockInsuficienteException(detalle.ProductoId);
            }

            producto.AjustarStock(-detalle.Cantidad);

            var movimiento = MovimientoInventario.CrearConsumoCampo(detalle.ProductoId, detalle.Cantidad, servicio.Id, comando.UsuarioId, fechaMovimiento);
            _movimientoInventarioRepository.Agregar(movimiento);
        }

        var idsDetallesOriginales = servicio.Detalles.Select(d => d.Id).ToHashSet();
        servicio.Cerrar(comando.Consumos.Select(c => (c.ProductoId, c.Cantidad)), comando.EstadoFinal, comando.Observaciones, comando.UsuarioId, fechaMovimiento);

        var detallesNuevos = servicio.Detalles.Where(d => !idsDetallesOriginales.Contains(d.Id));
        _servicioCampoRepository.AgregarDetalles(detallesNuevos);

        await _servicioCampoRepository.GuardarCambiosAsync(cancellationToken);

        return servicio.ADto();
    }
}
