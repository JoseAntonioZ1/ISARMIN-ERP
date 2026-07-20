using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.Taller.DTOs;
using ISARMIN.Domain.Entities.Taller;

namespace ISARMIN.Application.Modulos.Taller.Commands.RegistrarRecepcion;

/// <summary>UC-22/RF-051, RF-052 — RN-029: la recepción no es exclusiva de un rol.</summary>
public class RegistrarRecepcionCommandHandler : ICommandHandler<RegistrarRecepcionCommand, OrdenTrabajoDto>
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RegistrarRecepcionCommand> _validator;

    public RegistrarRecepcionCommandHandler(
        IOrdenTrabajoRepository ordenTrabajoRepository,
        IClienteRepository clienteRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<RegistrarRecepcionCommand> validator)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
        _clienteRepository = clienteRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<OrdenTrabajoDto> ManejarAsync(RegistrarRecepcionCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        if (await _clienteRepository.ObtenerPorIdAsync(comando.ClienteId, cancellationToken) is null)
        {
            throw new ClienteNoEncontradoException(comando.ClienteId);
        }

        var ot = new OrdenTrabajo(
            comando.ClienteId, comando.EquipoDescripcion, comando.FallaReportada, comando.UsuarioId, _fechaHoraProvider.UtcAhora);

        _ordenTrabajoRepository.Agregar(ot);
        await _ordenTrabajoRepository.GuardarCambiosAsync(cancellationToken);

        return ot.ADto();
    }
}
