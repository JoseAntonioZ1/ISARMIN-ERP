using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Clientes.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Clientes.Commands.EditarCliente;

public class EditarClienteCommandHandler : ICommandHandler<EditarClienteCommand, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<EditarClienteCommand> _validator;

    public EditarClienteCommandHandler(IClienteRepository clienteRepository, IValidator<EditarClienteCommand> validator)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public async Task<ClienteDto> ManejarAsync(EditarClienteCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var cliente = await _clienteRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new ClienteNoEncontradoException(comando.Id);

        TipoDocumento? tipoDocumento = comando.TipoDocumento is null ? null : Enum.Parse<TipoDocumento>(comando.TipoDocumento);

        cliente.ActualizarDatos(comando.NombreRazonSocial, comando.Telefono, comando.Direccion, tipoDocumento, comando.NumeroDocumento);
        await _clienteRepository.GuardarCambiosAsync(cancellationToken);

        return cliente.ADto();
    }
}
