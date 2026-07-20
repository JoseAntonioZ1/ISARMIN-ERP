using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.GenerarCotizacionReparacion;

/// <summary>UC-24/RF-055 — la OT debe estar en estado Diagnosticado.</summary>
public class GenerarCotizacionReparacionCommandHandler : ICommandHandler<GenerarCotizacionReparacionCommand, OrdenTrabajoDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<GenerarCotizacionReparacionCommand> _validator;

    public GenerarCotizacionReparacionCommandHandler(
        IOrdenTrabajoRepository ordenTrabajoRepository, IFechaHoraProvider fechaHoraProvider, IValidator<GenerarCotizacionReparacionCommand> validator)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<OrdenTrabajoDto> ManejarAsync(GenerarCotizacionReparacionCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ot = await _ordenTrabajoRepository.ObtenerPorIdAsync(comando.OrdenTrabajoId, cancellationToken)
            ?? throw new OrdenTrabajoNoEncontradaException(comando.OrdenTrabajoId);

        if (ot.Estado != EstadoOrdenTrabajo.Diagnosticado)
        {
            throw new EstadoOrdenTrabajoInvalidoException(ot.Id, "Solo se puede cotizar una OT en estado Diagnosticado.");
        }

        ot.GenerarCotizacion(comando.MontoEstimado, _fechaHoraProvider.UtcAhora);
        _ordenTrabajoRepository.AgregarCotizacion(ot.CotizacionReparacion!);
        await _ordenTrabajoRepository.GuardarCambiosAsync(cancellationToken);

        return ot.ADto();
    }
}
