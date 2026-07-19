using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;

namespace ISARMIN.Application.Modulos.Proveedores.Commands.CambiarEstadoProveedor;

public class CambiarEstadoProveedorCommandHandler : ICommandHandler<CambiarEstadoProveedorCommand, Unit>
{
    private readonly IProveedorRepository _proveedorRepository;

    public CambiarEstadoProveedorCommandHandler(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<Unit> ManejarAsync(CambiarEstadoProveedorCommand comando, CancellationToken cancellationToken = default)
    {
        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new ProveedorNoEncontradoException(comando.Id);

        if (comando.Activo)
        {
            proveedor.Activar();
        }
        else
        {
            proveedor.Desactivar();
        }

        await _proveedorRepository.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
