using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.EditarRol;

public class EditarRolCommandHandler : ICommandHandler<EditarRolCommand, RolDto>
{
    private readonly IRolRepository _rolRepository;
    private readonly IValidator<EditarRolCommand> _validator;

    public EditarRolCommandHandler(IRolRepository rolRepository, IValidator<EditarRolCommand> validator)
    {
        _rolRepository = rolRepository;
        _validator = validator;
    }

    public async Task<RolDto> ManejarAsync(EditarRolCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var rol = await _rolRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new RolNoEncontradoException(comando.Id);

        if (!string.Equals(rol.Nombre, comando.Nombre, StringComparison.Ordinal))
        {
            var existente = await _rolRepository.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
            if (existente is not null)
            {
                throw new NombreRolDuplicadoException(comando.Nombre);
            }
        }

        rol.ActualizarDatos(comando.Nombre, comando.Descripcion);
        await _rolRepository.GuardarCambiosAsync(cancellationToken);

        return rol.ADto();
    }
}
