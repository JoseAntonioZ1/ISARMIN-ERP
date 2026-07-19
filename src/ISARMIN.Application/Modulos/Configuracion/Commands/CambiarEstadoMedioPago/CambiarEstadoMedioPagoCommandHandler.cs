using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;

namespace ISARMIN.Application.Modulos.Configuracion.Commands.CambiarEstadoMedioPago;

public class CambiarEstadoMedioPagoCommandHandler : ICommandHandler<CambiarEstadoMedioPagoCommand, Unit>
{
    private readonly IMedioPagoRepository _medioPagoRepository;

    public CambiarEstadoMedioPagoCommandHandler(IMedioPagoRepository medioPagoRepository)
    {
        _medioPagoRepository = medioPagoRepository;
    }

    public async Task<Unit> ManejarAsync(CambiarEstadoMedioPagoCommand comando, CancellationToken cancellationToken = default)
    {
        var medioPago = await _medioPagoRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new MedioPagoNoEncontradoException(comando.Id);

        if (comando.Activo)
        {
            medioPago.Activar();
        }
        else
        {
            medioPago.Desactivar();
        }

        await _medioPagoRepository.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
