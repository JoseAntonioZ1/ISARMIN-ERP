using ISARMIN.Application.Modulos.Configuracion;
using ISARMIN.Domain.Entities.Configuracion;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class ConfiguracionEmpresaRepository : IConfiguracionEmpresaRepository
{
    private readonly IsarminDbContext _dbContext;

    public ConfiguracionEmpresaRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ConfiguracionEmpresa> ObtenerAsync(CancellationToken cancellationToken = default) =>
        _dbContext.ConfiguracionEmpresa.FirstAsync(cancellationToken);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
