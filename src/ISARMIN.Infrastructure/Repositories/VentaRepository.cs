using ISARMIN.Application.Modulos.Ventas;
using ISARMIN.Domain.Entities.Ventas;
using ISARMIN.Domain.Enums;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly IsarminDbContext _dbContext;

    public VentaRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Venta?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Ventas
            .Include(v => v.Detalles)
            .Include(v => v.Pagos)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<Venta> Ventas, int Total)> BuscarAsync(
        EstadoVenta? estado, Guid? clienteId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Ventas
            .Include(v => v.Detalles)
            .Include(v => v.Pagos)
            .AsQueryable();

        if (estado is { } estadoValor)
        {
            consulta = consulta.Where(v => v.Estado == estadoValor);
        }

        if (clienteId is { } clienteIdValor)
        {
            consulta = consulta.Where(v => v.ClienteId == clienteIdValor);
        }

        var total = await consulta.CountAsync(cancellationToken);

        var ventas = await consulta
            .OrderByDescending(v => v.Fecha)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (ventas, total);
    }

    public void Agregar(Venta venta) => _dbContext.Ventas.Add(venta);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
