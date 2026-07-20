using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarDiagnostico;

/// <summary>UC-23/RF-054 — la OT debe estar en estado Recibido.</summary>
public class RegistrarDiagnosticoCommandHandler : ICommandHandler<RegistrarDiagnosticoCommand, OrdenTrabajoDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RegistrarDiagnosticoCommand> _validator;

    public RegistrarDiagnosticoCommandHandler(
        IOrdenTrabajoRepository ordenTrabajoRepository, IFechaHoraProvider fechaHoraProvider, IValidator<RegistrarDiagnosticoCommand> validator)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<OrdenTrabajoDto> ManejarAsync(RegistrarDiagnosticoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ot = await _ordenTrabajoRepository.ObtenerPorIdAsync(comando.OrdenTrabajoId, cancellationToken)
            ?? throw new OrdenTrabajoNoEncontradaException(comando.OrdenTrabajoId);

        if (ot.Estado != EstadoOrdenTrabajo.Recibido)
        {
            throw new EstadoOrdenTrabajoInvalidoException(ot.Id, "Solo se puede registrar el diagnóstico cuando la OT está en estado Recibido.");
        }

        ot.RegistrarDiagnostico(comando.Descripcion, comando.UsuarioId, _fechaHoraProvider.UtcAhora);
        _ordenTrabajoRepository.AgregarDiagnostico(ot.Diagnostico!);
        await _ordenTrabajoRepository.GuardarCambiosAsync(cancellationToken);

        return ot.ADto();
    }
}
