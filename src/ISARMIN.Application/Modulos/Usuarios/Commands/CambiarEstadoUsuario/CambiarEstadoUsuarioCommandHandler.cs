using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.CambiarEstadoUsuario;

public class CambiarEstadoUsuarioCommandHandler : ICommandHandler<CambiarEstadoUsuarioCommand, Unit>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public CambiarEstadoUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Unit> ManejarAsync(CambiarEstadoUsuarioCommand comando, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new UsuarioNoEncontradoException(comando.Id);

        if (comando.Activo)
        {
            usuario.Activar();
        }
        else
        {
            usuario.Desactivar();
        }

        await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

        return Unit.Value;
    }
}
