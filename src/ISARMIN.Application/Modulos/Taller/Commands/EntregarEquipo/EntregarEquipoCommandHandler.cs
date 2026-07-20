using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.EntregarEquipo;

/// <summary>UC-26/RF-059 — RN-001: el pago no bloquea la entrega; RN-001/RN-031: el saldo pendiente
/// exige un usuario Administrador/Propietario autorizante. La OT debe estar en estado ListoParaEntrega.</summary>
public class EntregarEquipoCommandHandler : ICommandHandler<EntregarEquipoCommand, OrdenTrabajoDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<EntregarEquipoCommand> _validator;

    public EntregarEquipoCommandHandler(
        IOrdenTrabajoRepository ordenTrabajoRepository,
        IUsuarioRepository usuarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<EntregarEquipoCommand> validator)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _usuarioRepository = usuarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<OrdenTrabajoDto> ManejarAsync(EntregarEquipoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ot = await _ordenTrabajoRepository.ObtenerPorIdAsync(comando.OrdenTrabajoId, cancellationToken)
            ?? throw new OrdenTrabajoNoEncontradaException(comando.OrdenTrabajoId);

        if (ot.Estado != EstadoOrdenTrabajo.ListoParaEntrega)
        {
            throw new EstadoOrdenTrabajoInvalidoException(ot.Id, "Solo se puede entregar una OT en estado ListoParaEntrega.");
        }

        if (comando.UsuarioAutorizoSaldoId is { } usuarioAutorizoId
            && await _usuarioRepository.ObtenerPorIdAsync(usuarioAutorizoId, cancellationToken) is null)
        {
            throw new UsuarioNoEncontradoException(usuarioAutorizoId);
        }

        ot.EntregarEquipo(
            comando.EstadoPago, comando.MontoPagado, comando.SaldoPendiente,
            comando.UsuarioAutorizoSaldoId, comando.UsuarioEntregaId, _fechaHoraProvider.UtcAhora);

        await _ordenTrabajoRepository.GuardarCambiosAsync(cancellationToken);

        return ot.ADto();
    }
}
