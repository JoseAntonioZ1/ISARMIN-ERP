using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.IniciarSesion;

public class IniciarSesionCommandHandler : ICommandHandler<IniciarSesionCommand, SesionDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IGeneradorTokenJwt _generadorTokenJwt;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<IniciarSesionCommand> _validator;

    public IniciarSesionCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        IGeneradorTokenJwt generadorTokenJwt,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<IniciarSesionCommand> validator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _generadorTokenJwt = generadorTokenJwt;
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
        var token = _generadorTokenJwt.Generar(usuario, permisos);

        await _usuarioRepository.GuardarCambiosAsync(cancellationToken);

        return new SesionDto(
            token.Token,
            token.ExpiraEnUtc,
            new UsuarioSesionDto(usuario.Id, usuario.Nombre),
            permisos);
    }
}
