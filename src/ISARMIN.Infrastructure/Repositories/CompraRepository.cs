using ISARMIN.Application.Modulos.Compras;
using ISARMIN.Domain.Entities.Compras;
using ISARMIN.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ISARMIN.Infrastructure.Repositories;

public class CompraRepository : ICompraRepository
{
    private readonly IsarminDbContext _dbContext;

    public CompraRepository(IsarminDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Compra?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Compras.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<(IReadOnlyCollection<Compra> Compras, int Total)> BuscarAsync(
        Guid? proveedorId, Guid? productoId, int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var consulta = _dbContext.Compras.Include(c => c.Detalles).AsQueryable();

        if (proveedorId is { } idProveedor)
        {
            consulta = consulta.Where(c => c.ProveedorId == idProveedor);
        }

        if (productoId is { } idProducto)
        {
            consulta = consulta.Where(c => c.Detalles.Any(d => d.ProductoId == idProducto));
        }

        var total = await consulta.CountAsync(cancellationToken);

        var compras = await consulta
            .OrderByDescending(c => c.Fecha)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (compras, total);
    }

    public void Agregar(Compra compra) => _dbContext.Compras.Add(compra);

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
