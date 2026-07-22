using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.Commands.RegistrarProducto;

public class RegistrarProductoCommandHandler : ICommandHandler<RegistrarProductoCommand, ProductoDto>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnidadMedidaRepository _unidadMedidaRepository;
    private readonly IValidator<RegistrarProductoCommand> _validator;

    public RegistrarProductoCommandHandler(
        IProductoRepository productoRepository,
        ICategoriaRepository categoriaRepository,
        IUnidadMedidaRepository unidadMedidaRepository,
        IValidator<RegistrarProductoCommand> validator)
    {
        _productoRepository = productoRepository;
        _categoriaRepository = categoriaRepository;
        _unidadMedidaRepository = unidadMedidaRepository;
        _validator = validator;
    }

    public async Task<ProductoDto> ManejarAsync(RegistrarProductoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        if (await _productoRepository.ObtenerPorCodigoInternoAsync(comando.CodigoInterno, cancellationToken) is not null)
        {
            throw new CodigoInternoDuplicadoException(comando.CodigoInterno);
        }

        if (!string.IsNullOrWhiteSpace(comando.CodigoBarras)
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

        var producto = new Producto(
            comando.CodigoInterno,
            comando.Nombre,
            comando.CategoriaId,
            comando.UnidadMedidaId,
            comando.CostoReferencia,
            comando.PrecioVenta,
            comando.StockInicial,
            comando.Marca,
            comando.CodigoBarras,
            comando.StockMinimo,
            comando.Imagen);

        _productoRepository.Agregar(producto);
        await _productoRepository.GuardarCambiosAsync(cancellationToken);

        return producto.ADto();
    }
}
