using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObtenerPorHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    void Agregar(RefreshToken refreshToken);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
