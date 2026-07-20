using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarGarantia;

/// <summary>UC-27/RF-061 — RN-017 (alcance V1 acotado): la OT debe estar Entregada.</summary>
public class RegistrarGarantiaCommandHandler : ICommandHandler<RegistrarGarantiaCommand, GarantiaDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IGarantiaRepository _garantiaRepository;
    private readonly IValidator<RegistrarGarantiaCommand> _validator;

    public RegistrarGarantiaCommandHandler(
        IOrdenTrabajoRepository ordenTrabajoRepository, IGarantiaRepository garantiaRepository, IValidator<RegistrarGarantiaCommand> validator)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _garantiaRepository = garantiaRepository;
        _validator = validator;
    }

    public async Task<GarantiaDto> ManejarAsync(RegistrarGarantiaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ot = await _ordenTrabajoRepository.ObtenerPorIdAsync(comando.OrdenTrabajoId, cancellationToken)
            ?? throw new OrdenTrabajoNoEncontradaException(comando.OrdenTrabajoId);

        if (ot.Estado != EstadoOrdenTrabajo.Entregado)
        {
            throw new EstadoOrdenTrabajoInvalidoException(ot.Id, "Solo se puede registrar garantía sobre una OT entregada.");
        }

        if (await _garantiaRepository.ObtenerPorOrdenTrabajoIdAsync(ot.Id, cancellationToken) is not null)
        {
            throw new GarantiaYaRegistradaException(ot.Id);
        }

        var garantia = new Garantia(ot.Id, comando.FechaInicio, comando.FechaFin);

        _garantiaRepository.Agregar(garantia);
        await _garantiaRepository.GuardarCambiosAsync(cancellationToken);

        return garantia.ADto();
    }
}
