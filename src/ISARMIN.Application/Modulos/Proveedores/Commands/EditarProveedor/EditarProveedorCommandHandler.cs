using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Proveedores.DTOs;

namespace ISARMIN.Application.Modulos.Proveedores.Commands.EditarProveedor;

public class EditarProveedorCommandHandler : ICommandHandler<EditarProveedorCommand, ProveedorDto>
{
    private readonly IProveedorRepository _proveedorRepository;
    private readonly IValidator<EditarProveedorCommand> _validator;

    public EditarProveedorCommandHandler(IProveedorRepository proveedorRepository, IValidator<EditarProveedorCommand> validator)
    {
        _proveedorRepository = proveedorRepository;
        _validator = validator;
    }

    public async Task<ProveedorDto> ManejarAsync(EditarProveedorCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var proveedor = await _proveedorRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new ProveedorNoEncontradoException(comando.Id);

        proveedor.ActualizarDatos(comando.NombreRazonSocial, comando.Documento, comando.Telefono, comando.Direccion);
        await _proveedorRepository.GuardarCambiosAsync(cancellationToken);

        return proveedor.ADto();
    }
}
