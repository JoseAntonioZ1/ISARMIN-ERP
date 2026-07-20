using ISARMIN.Application.Modulos.Taller;
using ISARMIN.Domain.Entities.Taller;
using ISARMIN.Domain.Enums;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class OrdenTrabajoRepository : IOrdenTrabajoRepository
{
    private readonly IsarminDbContext _dbContext;

    public OrdenTrabajoRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<OrdenTrabajo?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.OrdenesTrabajo
            .Include(o => o.Diagnostico)
            .Include(o => o.CotizacionReparacion)
            .Include(o => o.ConsumosRepuesto)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<OrdenTrabajo> OrdenesTrabajo, int Total)> BuscarAsync(
        EstadoOrdenTrabajo? estado, Guid? clienteId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.OrdenesTrabajo
            .Include(o => o.Diagnostico)
            .Include(o => o.CotizacionReparacion)
            .Include(o => o.ConsumosRepuesto)
            .AsQueryable();

        if (estado is { } estadoValor)
        {
            consulta = consulta.Where(o => o.Estado == estadoValor);
        }

        if (clienteId is { } clienteIdValor)
        {
            consulta = consulta.Where(o => o.ClienteId == clienteIdValor);
        }

        var total = await consulta.CountAsync(cancellationToken);

        var ordenesTrabajo = await consulta
            .OrderByDescending(o => o.FechaRecepcion)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (ordenesTrabajo, total);
    }

    public void Agregar(OrdenTrabajo ordenTrabajo) => _dbContext.OrdenesTrabajo.Add(ordenTrabajo);

    public void AgregarDiagnostico(Diagnostico diagnostico) => _dbContext.Diagnosticos.Add(diagnostico);

    public void AgregarCotizacion(CotizacionReparacion cotizacion) => _dbContext.CotizacionesReparacion.Add(cotizacion);

    public void AgregarConsumosRepuesto(IEnumerable<ConsumoRepuesto> consumos) => _dbContext.ConsumosRepuesto.AddRange(consumos);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
