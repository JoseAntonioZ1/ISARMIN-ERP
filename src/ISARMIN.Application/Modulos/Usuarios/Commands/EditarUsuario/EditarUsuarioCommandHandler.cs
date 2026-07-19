using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.EditarUsuario;

public class EditarUsuarioCommandHandler : ICommandHandler<EditarUsuarioCommand, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IValidator<EditarUsuarioCommand> _validator;

    public EditarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IValidator<EditarUsuarioCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _validator = validator;
    }

    public async Task<UsuarioDto> ManejarAsync(EditarUsuarioCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new UsuarioNoEncontradoException(comando.Id);

        var rolesExistentes = await _rolRepository.ObtenerIdsExistentesAsync(comando.RolIds, cancellationToken);
        var rolIdInexistente = comando.RolIds.FirstOrDefault(id => !rolesExistentes.Contains(id));
        if (rolIdInexistente != Guid.Empty)
        {
            throw new RolInvalidoException(rolIdInexistente);
        }

        usuario.ActualizarNombre(comando.Nombre);
        usuario.ReemplazarRoles(comando.RolIds);

        await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

        var roles = (await _rolRepository.ListarTodosAsync(cancellationToken))
            .Where(r => comando.RolIds.Contains(r.Id))
            .Select(r => new RolResumenDto(r.Id, r.Nombre))
            .ToList();

        return new UsuarioDto(usuario.Id, usuario.Nombre, usuario.NombreUsuario, usuario.Estado.ToString(), roles);
    }
}
