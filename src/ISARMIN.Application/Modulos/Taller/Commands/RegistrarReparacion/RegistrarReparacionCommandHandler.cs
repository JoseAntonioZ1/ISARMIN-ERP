using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Domain.Entities.Inventario;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarReparacion;

/// <summary>UC-25/RF-057, RF-058 — RN-018: los repuestos consumidos se descuentan del inventario
/// compartido (RN-002, RN-006). La OT debe estar en estado Aprobado.</summary>
public class RegistrarReparacionCommandHandler : ICommandHandler<RegistrarReparacionCommand, OrdenTrabajoDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RegistrarReparacionCommand> _validator;

    public RegistrarReparacionCommandHandler(
        IOrdenTrabajoRepository ordenTrabajoRepository,
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoInventarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<RegistrarReparacionCommand> validator)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _productoRepository = productoRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<OrdenTrabajoDto> ManejarAsync(RegistrarReparacionCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ot = await _ordenTrabajoRepository.ObtenerPorIdAsync(comando.OrdenTrabajoId, cancellationToken)
            ?? throw new OrdenTrabajoNoEncontradaException(comando.OrdenTrabajoId);

        if (ot.Estado != EstadoOrdenTrabajo.Aprobado)
        {
            throw new EstadoOrdenTrabajoInvalidoException(ot.Id, "Solo se puede reparar una OT en estado Aprobado.");
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

            var movimiento = MovimientoInventario.CrearConsumoTaller(detalle.ProductoId, detalle.Cantidad, ot.Id, comando.UsuarioId, fechaMovimiento);
            _movimientoInventarioRepository.Agregar(movimiento);
        }

        var idsConsumosOriginales = ot.ConsumosRepuesto.Select(c => c.Id).ToHashSet();
        ot.RegistrarReparacion(comando.Consumos.Select(c => (c.ProductoId, c.Cantidad)), comando.ResultadoPruebas);

        var consumosNuevos = ot.ConsumosRepuesto.Where(c => !idsConsumosOriginales.Contains(c.Id));
        _ordenTrabajoRepository.AgregarConsumosRepuesto(consumosNuevos);

        await _ordenTrabajoRepository.GuardarCambiosAsync(cancellationToken);

        return ot.ADto();
    }
}
