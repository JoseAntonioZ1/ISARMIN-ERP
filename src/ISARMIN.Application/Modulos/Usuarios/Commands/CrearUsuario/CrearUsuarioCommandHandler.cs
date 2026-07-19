using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.CrearUsuario;

public class CrearUsuarioCommandHandler : ICommandHandler<CrearUsuarioCommand, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRolRepository _rolRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<CrearUsuarioCommand> _validator;

    public CrearUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IRolRepository rolRepository,
        IPasswordHasher passwordHasher,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<CrearUsuarioCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _rolRepository = rolRepository;
        _passwordHasher = passwordHasher;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<UsuarioDto> ManejarAsync(CrearUsuarioCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var existente = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(comando.NombreUsuario, cancellationToken);
        if (existente is not null)
        {
            throw new NombreUsuarioDuplicadoException(comando.NombreUsuario);
        }

        var rolesExistentes = await _rolRepository.ObtenerIdsExistentesAsync(comando.RolIds, cancellationToken);
        var rolIdInexistente = comando.RolIds.FirstOrDefault(id => !rolesExistentes.Contains(id));
        if (rolIdInexistente != Guid.Empty)
        {
            throw new RolInvalidoException(rolIdInexistente);
        }

        var credencialHash = _passwordHasher.HashearCredencial(comando.CredencialInicial);
        var usuario = new Usuario(comando.Nombre, comando.NombreUsuario, credencialHash, _fechaHoraProvider.UtcAhora);

        foreach (var rolId in comando.RolIds)
        {
            usuario.AsignarRol(rolId);
        }

        _usuarioRepository.Agregar(usuario);
        await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

        var roles = (await _rolRepository.ListarTodosAsync(cancellationToken))
            .Where(r => comando.RolIds.Contains(r.Id))
            .Select(r => new RolResumenDto(r.Id, r.Nombre))
            .ToList();

        return new UsuarioDto(usuario.Id, usuario.Nombre, usuario.NombreUsuario, usuario.Estado.ToString(), roles);
    }
}
