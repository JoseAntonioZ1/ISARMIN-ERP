using ISARMIN.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ISARMIN.Infrastructure.Salud;

public class PostgresHealthCheck(IsarminDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var puedeConectar = await dbContext.Database.CanConnectAsync(cancellationToken);

        return puedeConectar
            ? HealthCheckResult.Healthy("Conexión a PostgreSQL correcta.")
            : HealthCheckResult.Unhealthy("No se pudo conectar a PostgreSQL.");
    }
}
