using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;

namespace ISARMIN.Application.Modulos.Clientes.Commands.CambiarEstadoCliente;

public class CambiarEstadoClienteCommandHandler : ICommandHandler<CambiarEstadoClienteCommand, Unit>
{
    private readonly IClienteRepository _clienteRepository;

    public CambiarEstadoClienteCommandHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Unit> ManejarAsync(CambiarEstadoClienteCommand comando, CancellationToken cancellationToken = default)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new ClienteNoEncontradoException(comando.Id);

        if (comando.Activo)
        {
            cliente.Activar();
        }
        else
        {
            cliente.Desactivar();
        }

        await _clienteRepository.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
