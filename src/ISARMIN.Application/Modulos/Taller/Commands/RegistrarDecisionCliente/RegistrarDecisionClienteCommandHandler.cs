using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarDecisionCliente;

/// <summary>UC-24/RF-056 — RN-016/RN-030: la OT debe estar Cotizada; el cobro por diagnóstico solo
/// aplica si el cliente rechaza.</summary>
public class RegistrarDecisionClienteCommandHandler : ICommandHandler<RegistrarDecisionClienteCommand, OrdenTrabajoDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IValidator<RegistrarDecisionClienteCommand> _validator;

    public RegistrarDecisionClienteCommandHandler(
        IOrdenTrabajoRepository ordenTrabajoRepository, IValidator<RegistrarDecisionClienteCommand> validator)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _validator = validator;
    }

    public async Task<OrdenTrabajoDto> ManejarAsync(RegistrarDecisionClienteCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ot = await _ordenTrabajoRepository.ObtenerPorIdAsync(comando.OrdenTrabajoId, cancellationToken)
            ?? throw new OrdenTrabajoNoEncontradaException(comando.OrdenTrabajoId);

        if (ot.Estado != EstadoOrdenTrabajo.Cotizado)
        {
            throw new EstadoOrdenTrabajoInvalidoException(ot.Id, "Solo se puede registrar la decisión del cliente sobre una OT en estado Cotizado.");
        }

        ot.RegistrarDecisionCliente(comando.Decision, comando.CobroDiagnosticoRechazo, comando.EvidenciaAprobacion);
        await _ordenTrabajoRepository.GuardarCambiosAsync(cancellationToken);

        return ot.ADto();
    }
}
