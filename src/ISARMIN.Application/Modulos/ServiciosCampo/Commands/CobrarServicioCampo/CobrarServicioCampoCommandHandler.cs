using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.CobrarServicioCampo;

/// <summary>UC-33/RF-070 — RN-031: el saldo pendiente exige un usuario Administrador/Propietario
/// autorizante. El servicio debe estar en estado Cerrado.</summary>
public class CobrarServicioCampoCommandHandler : ICommandHandler<CobrarServicioCampoCommand, ServicioCampoDto>
{
    private readonly IServicioCampoRepository _servicioCampoRepository;
    private readonly IMedioPagoRepository _medioPagoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IValidator<CobrarServicioCampoCommand> _validator;

    public CobrarServicioCampoCommandHandler(
        IServicioCampoRepository servicioCampoRepository,
        IMedioPagoRepository medioPagoRepository,
        IUsuarioRepository usuarioRepository,
        IValidator<CobrarServicioCampoCommand> validator)
    {
        _servicioCampoRepository = servicioCampoRepository;
        _medioPagoRepository = medioPagoRepository;
        _usuarioRepository = usuarioRepository;
        _validator = validator;
    }

    public async Task<ServicioCampoDto> ManejarAsync(CobrarServicioCampoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var servicio = await _servicioCampoRepository.ObtenerPorIdAsync(comando.ServicioCampoId, cancellationToken)
            ?? throw new ServicioCampoNoEncontradoException(comando.ServicioCampoId);

        if (servicio.Estado != EstadoServicioCampo.Cerrado)
        {
            throw new EstadoServicioCampoInvalidoException(servicio.Id, "Solo se puede registrar el cobro de un servicio en estado Cerrado.");
        }

        if (await _medioPagoRepository.ObtenerPorIdAsync(comando.MedioPagoId, cancellationToken) is null)
        {
            throw new MedioPagoNoEncontradoException(comando.MedioPagoId);
        }

        if (comando.UsuarioAutorizoSaldoId is { } usuarioAutorizoId
            && await _usuarioRepository.ObtenerPorIdAsync(usuarioAutorizoId, cancellationToken) is null)
        {
            throw new UsuarioNoEncontradoException(usuarioAutorizoId);
        }

        servicio.RegistrarCobro(comando.MedioPagoId, comando.MontoPagado, comando.SaldoPendiente, comando.UsuarioAutorizoSaldoId);

        await _servicioCampoRepository.GuardarCambiosAsync(cancellationToken);

        return servicio.ADto();
    }
}
