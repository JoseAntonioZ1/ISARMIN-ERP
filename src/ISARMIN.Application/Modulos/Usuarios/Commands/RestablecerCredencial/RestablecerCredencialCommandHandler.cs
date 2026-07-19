using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.RestablecerCredencial;

public class RestablecerCredencialCommandHandler : ICommandHandler<RestablecerCredencialCommand, Unit>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RestablecerCredencialCommand> _validator;

    public RestablecerCredencialCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IValidator<RestablecerCredencialCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
    }

    public async Task<Unit> ManejarAsync(RestablecerCredencialCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new UsuarioNoEncontradoException(comando.Id);

        var nuevoHash = _passwordHasher.HashearCredencial(comando.NuevaCredencial);
        usuario.RestablecerCredencial(nuevoHash);

        await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
