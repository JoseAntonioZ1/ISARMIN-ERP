using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.CrearRol;

public class CrearRolCommandHandler : ICommandHandler<CrearRolCommand, RolDto>
{
    private readonly IRolRepository _rolRepository;
    private readonly IValidator<CrearRolCommand> _validator;

    public CrearRolCommandHandler(IRolRepository rolRepository, IValidator<CrearRolCommand> validator)
    {
        _rolRepository = rolRepository;
        _validator = validator;
    }

    public async Task<RolDto> ManejarAsync(CrearRolCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var existente = await _rolRepository.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
        if (existente is not null)
        {
            throw new NombreRolDuplicadoException(comando.Nombre);
        }

        var rol = new Rol(comando.Nombre, comando.Descripcion);
        _rolRepository.Agregar(rol);
        await _rolRepository.GuardarCambiosAsync(cancellationToken);

        return rol.ADto();
    }
}
