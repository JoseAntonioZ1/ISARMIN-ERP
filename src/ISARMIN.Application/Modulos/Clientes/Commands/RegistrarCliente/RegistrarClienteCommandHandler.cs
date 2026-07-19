using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Clientes.DTOs;
using ISARMIN.Domain.Entities.Terceros;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Clientes.Commands.RegistrarCliente;

public class RegistrarClienteCommandHandler : ICommandHandler<RegistrarClienteCommand, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<RegistrarClienteCommand> _validator;

    public RegistrarClienteCommandHandler(IClienteRepository clienteRepository, IValidator<RegistrarClienteCommand> validator)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public async Task<ClienteDto> ManejarAsync(RegistrarClienteCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        TipoDocumento? tipoDocumento = comando.TipoDocumento is null ? null : Enum.Parse<TipoDocumento>(comando.TipoDocumento);

        var cliente = new Cliente(comando.NombreRazonSocial, comando.Telefono, comando.Direccion, tipoDocumento, comando.NumeroDocumento);

        _clienteRepository.Agregar(cliente);
        await _clienteRepository.GuardarCambiosAsync(cancellationToken);

        return cliente.ADto();
    }
}
