using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Inventario.Commands.EditarProducto;

public class EditarProductoCommandHandler : ICommandHandler<EditarProductoCommand, ProductoDto>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;
    private readonly IValidator<EditarProductoCommand> _validator;

    public EditarProductoCommandHandler(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IUnidadMedidaRepository unidadMedidaRepository,
        IValidator<EditarProductoCommand> validator)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _unidadMedidaRepository = unidadMedidaRepository;
        _validator = validator;
    }

    public async Task<ProductoDto> ManejarAsync(EditarProductoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var producto = await _productoRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new ProductoNoEncontradoException(comando.Id);

        if (!string.Equals(producto.CodigoInterno, comando.CodigoInterno, StringComparison.Ordinal))
        {
            if (await _productoRepository.ObtenerPorCodigoInternoAsync(comando.CodigoInterno, cancellationToken) is not null)
            {
                throw new CodigoInternoDuplicadoException(comando.CodigoInterno);
            }
        }

        if (!string.IsNullOrWhiteSpace(comando.CodigoBarras)
            && !string.Equals(producto.CodigoBarras, comando.CodigoBarras, StringComparison.Ordinal)
            && await _productoRepository.ObtenerPorCodigoBarrasAsync(comando.CodigoBarras, cancellationToken) is not null)
        {
            throw new CodigoBarrasDuplicadoException(comando.CodigoBarras);
        }

        if (!await _categoriaRepository.ExisteAsync(comando.CategoriaId, cancellationToken))
        {
            throw new CategoriaInvalidaException(comando.CategoriaId);
        }

        if (!await _unidadMedidaRepository.ExisteAsync(comando.UnidadMedidaId, cancellationToken))
        {
            throw new UnidadMedidaInvalidaException(comando.UnidadMedidaId);
        }

        producto.ActualizarDatos(
            comando.CodigoInterno,
            comando.Nombre,
            comando.CategoriaId,
            comando.UnidadMedidaId,
            comando.CostoReferencia,
            comando.PrecioVenta,
            comando.Marca,
            comando.CodigoBarras,
            comando.StockMinimo);

        await _productoRepository.GuardarCambiosAsync(cancellationToken);

        return producto.ADto();
    }
}
