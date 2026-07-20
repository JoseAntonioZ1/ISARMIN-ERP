using ISARMIN.Application.Modulos.ServiciosCampo;
using ISARMIN.Domain.Entities.ServiciosCampo;
using ISARMIN.Domain.Enums;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class ServicioCampoRepository : IServicioCampoRepository
{
    private readonly IsarminDbContext _dbContext;

    public ServicioCampoRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ServicioCampo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.ServiciosCampo
            .Include(s => s.Detalles)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<ServicioCampo> Servicios, int Total)> BuscarAsync(
        EstadoServicioCampo? estado, Guid? clienteId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.ServiciosCampo
            .Include(s => s.Detalles)
            .AsQueryable();

        if (estado is { } estadoValor)
        {
            consulta = consulta.Where(s => s.Estado == estadoValor);
        }

        if (clienteId is { } clienteIdValor)
        {
            consulta = consulta.Where(s => s.ClienteId == clienteIdValor);
        }

        var total = await consulta.CountAsync(cancellationToken);

        var servicios = await consulta
            .OrderByDescending(s => s.FechaSolicitud)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (servicios, total);
    }

    public void Agregar(ServicioCampo servicioCampo) => _dbContext.ServiciosCampo.Add(servicioCampo);

    public void AgregarDetalles(IEnumerable<ServicioCampoDetalle> detalles) => _dbContext.ServiciosCampoDetalle.AddRange(detalles);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
