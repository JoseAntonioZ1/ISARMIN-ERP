using ISARMIN.Application.Common;
using ISARMIN.Application.Modulos.Usuarios.DTOs;
using ISARMIN.Domain.Entities.Identidad;

namespace ISARMIN.Application.Modulos.Usuarios.Servicios;

/// <summary>
/// Emite el JWT de acceso junto con un nuevo refresh token, usado tanto al iniciar sesión
/// como al renovarla — evita duplicar la lógica de emisión en ambos handlers.
/// </summary>
public class EmisorSesion
{
    private const int RefreshTokenDuracionDias = 7;

    private readonly IGeneradorTokenJwt _generadorTokenJwt;
    private readonly IGeneradorTokenOpaco _generadorTokenOpaco;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public EmisorSesion(
        IGeneradorTokenJwt generadorTokenJwt,
        IGeneradorTokenOpaco generadorTokenOpaco,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _generadorTokenJwt = generadorTokenJwt;
        _generadorTokenOpaco = generadorTokenOpaco;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public SesionDto Emitir(Usuario usuario, IReadOnlyCollection<string> permisos, DateTime ahora)
    {
        var token = _generadorTokenJwt.Generar(usuario, permisos);

        var refreshTokenCrudo = _generadorTokenOpaco.Generar();
        var refreshTokenExpiraEn = ahora.AddDays(RefreshTokenDuracionDias);
        _refreshTokenRepository.Agregar(new RefreshToken(
            usuario.Id, HashSha256.Calcular(refreshTokenCrudo), ahora, refreshTokenExpiraEn));

        return new SesionDto(
            token.Token,
            token.ExpiraEnUtc,
            refreshTokenCrudo,
            refreshTokenExpiraEn,
            new UsuarioSesionDto(usuario.Id, usuario.Nombre),
            permisos);
    }
}
