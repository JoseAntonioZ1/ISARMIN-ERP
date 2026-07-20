using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Clientes;
using ISARMIN.Application.Modulos.ServiciosCampo.DTOs;
using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Entities.ServiciosCampo;

namespace ISARMIN.Application.Modulos.ServiciosCampo.Commands.SolicitarServicioCampo;

/// <summary>UC-30/RF-064, RF-065 — BQ-037: la asignación de técnico es opcional/informal.</summary>
public class SolicitarServicioCampoCommandHandler : ICommandHandler<SolicitarServicioCampoCommand, ServicioCampoDto>
{
    private readonly IServicioCampoRepository _servicioCampoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<SolicitarServicioCampoCommand> _validator;

    public SolicitarServicioCampoCommandHandler(
        IServicioCampoRepository servicioCampoRepository,
        IClienteRepository clienteRepository,
        IUsuarioRepository usuarioRepository,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<SolicitarServicioCampoCommand> validator)
    {
        _servicioCampoRepository = servicioCampoRepository;
        _clienteRepository = clienteRepository;
        _usuarioRepository = usuarioRepository;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<ServicioCampoDto> ManejarAsync(SolicitarServicioCampoCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        if (await _clienteRepository.ObtenerPorIdAsync(comando.ClienteId, cancellationToken) is null)
        {
            throw new ClienteNoEncontradoException(comando.ClienteId);
        }

        if (comando.TecnicoAsignadoId is { } tecnicoId && await _usuarioRepository.ObtenerPorIdAsync(tecnicoId, cancellationToken) is null)
        {
            throw new UsuarioNoEncontradoException(tecnicoId);
        }

        var servicio = new ServicioCampo(comando.ClienteId, comando.DescripcionTrabajo, comando.TecnicoAsignadoId, _fechaHoraProvider.UtcAhora);

        _servicioCampoRepository.Agregar(servicio);
        await _servicioCampoRepository.GuardarCambiosAsync(cancellationToken);

        return servicio.ADto();
    }
}
