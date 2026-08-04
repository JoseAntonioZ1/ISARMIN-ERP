using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Application.Modulos.Usuarios.Servicios;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;

public class IniciarSesionCommandHandler : ICommandHandler<IniciarSesionCommand, SesionDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly EmisorSesion _emisorSesion;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<IniciarSesionCommand> _validator;

    public IniciarSesionCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        EmisorSesion emisorSesion,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<IniciarSesionCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _emisorSesion = emisorSesion;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<SesionDto> ManejarAsync(IniciarSesionCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ahora = _fechaHoraProvider.UtcAhora;

        var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(comando.NombreUsuario, cancellationToken);
        if (usuario is null)
        {
            throw new CredencialesInvalidasException();
        }

        if (usuario.EstaBloqueado(ahora))
        {
            throw new CuentaBloqueadaException(usuario.BloqueadoHasta!.Value);
        }

        // RN-021: un usuario inactivo no puede autenticarse. Se responde igual que
        // credenciales inválidas para no revelar el estado de la cuenta a quien intenta acceder.
        if (!usuario.EstaActivo)
        {
            throw new CredencialesInvalidasException();
        }

        if (!_passwordHasher.VerificarCredencial(usuario.CredencialHash, comando.Credencial))
        {
            usuario.RegistrarIntentoFallido(ahora);
            await _usuarioRepository.GuardarCambiosAsync(cancellationToken);
            throw new CredencialesInvalidasException();
        }

        usuario.RegistrarInicioSesionExitoso();

        var permisos = await _usuarioRepository.ObtenerPermisosEfectivosAsync(usuario.Id, cancellationToken);
        var sesion = _emisorSesion.Emitir(usuario, permisos, ahora);

        await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

        return sesion;
    }
}
