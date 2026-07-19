using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.AsignarPermisos;

public class AsignarPermisosCommandHandler : ICommandHandler<AsignarPermisosCommand, RolDto>
{
    private readonly IRolRepository _rolRepository;
    private readonly IValidator<AsignarPermisosCommand> _validator;

    public AsignarPermisosCommandHandler(IRolRepository rolRepository, IValidator<AsignarPermisosCommand> validator)
    {
        _rolRepository = rolRepository;
        _validator = validator;
    }

    public async Task<RolDto> ManejarAsync(AsignarPermisosCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var rol = await _rolRepository.ObtenerPorIdAsync(comando.RolId, cancellationToken)
            ?? throw new RolNoEncontradoException(comando.RolId);

        // Permiso.Id ya viene asignado (Guid.NewGuid()) antes de que EF Core lo vea, así que no
        // puede reconocer un Permiso nuevo por "clave aún no asignada" cuando se agrega solo
        // mutando la colección de un Rol ya rastreado (a diferencia de un Rol/Usuario nuevo,
        // que se agrega completo vía Add()). Se registran explícitamente los que son realmente
        // nuevos para que EF los inserte en vez de intentar un UPDATE inexistente.
        var idsOriginales = rol.Permisos.Select(p => p.Id).ToHashSet();

        var permisos = comando.Permisos.Select(p => (p.Modulo, Enum.Parse<AccionPermiso>(p.Accion)));
        rol.ReemplazarPermisos(permisos);

        var permisosNuevos = rol.Permisos.Where(p => !idsOriginales.Contains(p.Id));
        _rolRepository.AgregarPermisos(permisosNuevos);

        await _rolRepository.GuardarCambiosAsync(cancellationToken);

        return rol.ADto();
    }
}
