using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.Commands.AjustarInventario;

/// <summary>UC-12/RF-030 — ajuste manual de inventario, exclusivo del Administrador (RN-008).</summary>
public class AjustarInventarioCommandHandler : ICommandHandler<AjustarInventarioCommand, ProductoDto>
{
    private readonly IProductoRepository _productoRepository;
    private readonly IMovimientoInventarioRepository _movimientoInventarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<AjustarInventarioCommand> _validator;

    public AjustarInventarioCommandHandler(
        IProductoRepository productoRepository,
        IMovimientoInventarioRepository movimientoInventarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<AjustarInventarioCommand> validator)
    {
        _productoRepository = productoRepository;
        _movimientoInventarioRepository = movimientoInventarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<ProductoDto> ManejarAsync(AjustarInventarioCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var producto = await _productoRepository.ObtenerPorIdAsync(comando.ProductoId, cancellationToken)
            ?? throw new ProductoNoEncontradoException(comando.ProductoId);

        if (producto.StockActual + comando.CantidadAjuste < 0)
        {
            throw new AjusteInventarioInvalidoException(producto.Id);
        }

        var fecha = _fechaHoraProvider.UtcAhora;
        var movimiento = MovimientoInventario.CrearAjuste(producto.Id, comando.CantidadAjuste, comando.Motivo, comando.UsuarioId, fecha);

        producto.AjustarStock(comando.CantidadAjuste);
        _movimientoInventarioRepository.Agregar(movimiento);

        await _productoRepository.GuardarCambiosAsync(cancellationToken);

        return producto.ADto();
    }
}
