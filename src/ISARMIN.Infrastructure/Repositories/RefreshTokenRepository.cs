using ISARMIN.Application.Modulos.Usuarios;
using ISARMIN.Domain.Entities.Identidad;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IsarminDbContext _dbContext;

    public RefreshTokenRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RefreshToken?> ObtenerPorHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

    public void Agregar(RefreshToken refreshToken) => _dbContext.RefreshTokens.Add(refreshToken);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
