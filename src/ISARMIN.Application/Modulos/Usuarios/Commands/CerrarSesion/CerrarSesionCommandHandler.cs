using ISARMIN.Application.Common;

namespace ISARMIN.Application.Modulos.Usuarios.Commands.CerrarSesion;

/// <summary>
/// El JWT en sí es sin estado y no se puede invalidar, pero sí podemos revocar el refresh
/// token asociado para que la sesión no pueda renovarse indefinidamente tras el logout.
/// </summary>
public class CerrarSesionCommandHandler : ICommandHandler<CerrarSesionCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IFechaHoraProvider _fechaHoraProvider;

    public CerrarSesionCommandHandler(IRefreshTokenRepository refreshTokenRepository, IFechaHoraProvider fechaHoraProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _fechaHoraProvider = fechaHoraProvider;
    }

    public async Task<Unit> ManejarAsync(CerrarSesionCommand comando, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(comando.RefreshToken))
        {
            var tokenHash = HashSha256.Calcular(comando.RefreshToken);
            var refreshToken = await _refreshTokenRepository.ObtenerPorHashAsync(tokenHash, cancellationToken);
            refreshToken?.Revocar(_fechaHoraProvider.UtcAhora);
            await _refreshTokenRepository.GuardarCambiosAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
