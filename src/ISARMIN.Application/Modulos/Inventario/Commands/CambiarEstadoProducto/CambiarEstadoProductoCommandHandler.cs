using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;

namespace ISARMIN.Application.Modulos.Inventario.Commands.CambiarEstadoProducto;

public class CambiarEstadoProductoCommandHandler : ICommandHandler<CambiarEstadoProductoCommand, Unit>
{
    private readonly IProductoRepository _productoRepository;

    public CambiarEstadoProductoCommandHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<Unit> ManejarAsync(CambiarEstadoProductoCommand comando, CancellationToken cancellationToken = default)
    {
        var producto = await _productoRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new ProductoNoEncontradoException(comando.Id);

        if (comando.Activo)
        {
            producto.Activar();
        }
        else
        {
            producto.Desactivar();
        }

        await _productoRepository.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
