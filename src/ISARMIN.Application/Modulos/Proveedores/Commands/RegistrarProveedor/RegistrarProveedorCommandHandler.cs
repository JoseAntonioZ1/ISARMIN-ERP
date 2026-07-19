using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Proveedores.DTOs;
using ISARMIN.Domain.Entities.Terceros;

namespace ISARMIN.Application.Modulos.Proveedores.Commands.RegistrarProveedor;

public class RegistrarProveedorCommandHandler : ICommandHandler<RegistrarProveedorCommand, ProveedorDto>
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IValidator<RegistrarProveedorCommand> _validator;

    public RegistrarProveedorCommandHandler(IProveedorRepository proveedorRepository, IValidator<RegistrarProveedorCommand> validator)
    {
        _proveedorRepository = proveedorRepository;
        _validator = validator;
    }

    public async Task<ProveedorDto> ManejarAsync(RegistrarProveedorCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var proveedor = new Proveedor(comando.NombreRazonSocial, comando.Documento, comando.Telefono, comando.Direccion);

        _proveedorRepository.Agregar(proveedor);
        await _proveedorRepository.GuardarCambiosAsync(cancellationToken);

        return proveedor.ADto();
    }
}
