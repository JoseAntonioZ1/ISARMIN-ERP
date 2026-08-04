using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Application.Modulos.Usuarios.Servicios;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.RenovarSesion;

public class RenovarSesionCommandHandler : ICommandHandler<RenovarSesionCommand, SesionDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly EmisorSesion _emisorSesion;
    private readonly IFechaHoraProvider _fechaHoraProvider;
    private readonly IValidator<RenovarSesionCommand> _validator;

    public RenovarSesionCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUsuarioRepository usuarioRepository,
        EmisorSesion emisorSesion,
        IFechaHoraProvider fechaHoraProvider,
        IValidator<RenovarSesionCommand> validator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _usuarioRepository = usuarioRepository;
        _emisorSesion = emisorSesion;
        _fechaHoraProvider = fechaHoraProvider;
        _validator = validator;
    }

    public async Task<SesionDto> ManejarAsync(RenovarSesionCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var ahora = _fechaHoraProvider.UtcAhora;
        var tokenHash = HashSha256.Calcular(comando.RefreshToken);

        var refreshTokenActual = await _refreshTokenRepository.ObtenerPorHashAsync(tokenHash, cancellationToken);
        if (refreshTokenActual is null || !refreshTokenActual.EsValido(ahora))
        {
            throw new RefreshTokenInvalidoException();
        }

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(refreshTokenActual.UsuarioId, cancellationToken);
        if (usuario is null || !usuario.EstaActivo)
        {
            throw new RefreshTokenInvalidoException();
        }

        // Rotación: se revoca el token usado y se emite uno nuevo. Si un refresh token filtrado
        // fuera usado por un tercero, el uso legítimo siguiente ya lo encontraría revocado.
        refreshTokenActual.Revocar(ahora);

        var permisos = await _usuarioRepository.ObtenerPermisosEfectivosAsync(usuario.Id, cancellationToken);
        var sesion = _emisorSesion.Emitir(usuario, permisos, ahora);

        await _refreshTokenRepository.GuardarCambiosAsync(cancellationToken);

        return sesion;
    }
}
